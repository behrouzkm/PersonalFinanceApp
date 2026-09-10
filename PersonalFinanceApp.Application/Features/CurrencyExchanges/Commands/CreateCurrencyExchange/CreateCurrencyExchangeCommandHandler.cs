using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Application.Features.CurrencyExchanges.Commands.CreateCurrencyExchange;

public class CreateCurrencyExchangeCommandHandler : IRequestHandler<CreateCurrencyExchangeCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAccountingLookupService _lookupService;
    private readonly ILedgerBalanceValidationService _ledgerValidator;

    public CreateCurrencyExchangeCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser,
        IAccountingLookupService lookupService, ILedgerBalanceValidationService ledgerValidator)
    {
        _context = context;
        _currentUser = currentUser;
        _lookupService = lookupService;
        _ledgerValidator = ledgerValidator;
    }


    public async Task<Guid> Handle(CreateCurrencyExchangeCommand request, CancellationToken cancellationToken)
    {
        var (fromFundSource, fromLedgerAccount) = await _lookupService
                .GetFundSourceByLedgerAccountIdAsync(request.FromLedgerAccountId, cancellationToken);


        var (toFundSource, toLedgerAccount) = await _lookupService
                .GetFundSourceByLedgerAccountIdAsync(request.ToLedgerAccountId, cancellationToken);


        if (fromFundSource.CurrencyId == toFundSource.CurrencyId)
            throw new BusinessRuleException(ApplicationErrorCodes.CurrencyExchange.SameCurrencyExchangeNotAllowed);

        if (!fromFundSource.CanWithdraw(request.FromAmount))
            throw new BusinessRuleException(ApplicationErrorCodes.CurrencyExchange.InsufficientBalance);

        await _ledgerValidator.ValidateAsync(fromFundSource, request.ExchangeDate, 0, request.FromAmount,
                replacingEntryId: null, cancellationToken);


        var fromClearing = await _lookupService
                .GetOrCreateCurrencyExchangeClearingLedgerAccountAsync(fromFundSource.CurrencyId, cancellationToken);

        var toClearing = await _lookupService
                .GetOrCreateCurrencyExchangeClearingLedgerAccountAsync(toFundSource.CurrencyId, cancellationToken);


        var fromDocumentType = DocumentType.CurrencyExchange;
        var toDocumentType = DocumentType.CurrencyExchange;


        var fromDocument = new AccountingDocument(fromDocumentType, request.ExchangeDate, fromFundSource.CurrencyId,
                    _currentUser.TenantId, _currentUser.UserId, request.Description);

        fromDocument.AddEntry(fromClearing.Id, request.FromAmount, 0, request.Description, _currentUser.UserId);
        fromDocument.AddEntry(fromFundSource.LedgerAccountId, 0, request.FromAmount, request.Description, _currentUser.UserId);


        var toDocument = new AccountingDocument(toDocumentType, request.ExchangeDate, toFundSource.CurrencyId,
                    _currentUser.TenantId, _currentUser.UserId, request.Description);

        toDocument.AddEntry(toFundSource.LedgerAccountId, request.ToAmount, 0, request.Description, _currentUser.UserId);
        toDocument.AddEntry(toClearing.Id, 0, request.ToAmount, request.Description, _currentUser.UserId);


        fromDocument.EnsureBalanced();
        toDocument.EnsureBalanced();


        fromLedgerAccount.MarkAsUsed();
        toLedgerAccount.MarkAsUsed();
        fromClearing.MarkAsUsed();
        toClearing.MarkAsUsed();

        fromFundSource.AdjustBalance(-request.FromAmount);
        toFundSource.AdjustBalance(request.ToAmount);


        var exchange = new CurrencyExchange(fromDocument.Id, toDocument.Id, request.ExchangeRate, _currentUser.TenantId,
                _currentUser.UserId, request.Description);

        _context.AccountingDocuments.AddRange(fromDocument, toDocument);
        _context.CurrencyExchanges.Add(exchange);

        await _context.SaveChangesAsync(cancellationToken);

        return exchange.Id;
    }
}
