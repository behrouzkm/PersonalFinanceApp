using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.CurrencyExchanges.Commands.RestoreCurrencyExchange;

public class RestoreCurrencyExchangeCommandHandler : IRequestHandler<RestoreCurrencyExchangeCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IAccountingLookupService _lookupService;
    private readonly ICurrentUserService _currentUser;
    private readonly IAttachmentService _attachmentService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILedgerBalanceValidationService _ledgerValidator;

    public RestoreCurrencyExchangeCommandHandler(
            IApplicationDbContext context,
            IAccountingLookupService lookupService,
            ICurrentUserService currentUser,
            IAttachmentService attachmentService,
            IUnitOfWork unitOfWork,
            ILedgerBalanceValidationService ledgerValidator)
    {
        _context = context;
        _lookupService = lookupService;
        _currentUser = currentUser;
        _attachmentService = attachmentService;
        _unitOfWork = unitOfWork;
        _ledgerValidator = ledgerValidator;
    }

    public async Task Handle(RestoreCurrencyExchangeCommand request, CancellationToken cancellationToken)
    {
        var exchange = await _context.CurrencyExchanges
            .IgnoreQueryFilters()
            .Include(e => e.FromDocument).ThenInclude(d => d.Entries)
            .Include(e => e.ToDocument).ThenInclude(d => d.Entries)
            .FirstOrDefaultAsync(e => e.Id == request.CurrencyExchangeId && e.TenantId == _currentUser.TenantId, cancellationToken)
            ?? throw new NotFoundException(nameof(CurrencyExchange), request.CurrencyExchangeId);

        var fromClearing = await _lookupService
            .GetOrCreateCurrencyExchangeClearingLedgerAccountAsync(exchange.FromDocument.CurrencyId, cancellationToken);

        var toClearing = await _lookupService
            .GetOrCreateCurrencyExchangeClearingLedgerAccountAsync(exchange.ToDocument.CurrencyId, cancellationToken);

        var fromEntry = exchange.FromDocument.Entries.First(e => e.LedgerAccountId != fromClearing.Id);
        var toEntry = exchange.ToDocument.Entries.First(e => e.LedgerAccountId != toClearing.Id);

        var (fromFundSource, _) = await _lookupService
            .GetFundSourceByLedgerAccountIdAsync(fromEntry.LedgerAccountId, cancellationToken);
        var (toFundSource, _) = await _lookupService
            .GetFundSourceByLedgerAccountIdAsync(toEntry.LedgerAccountId, cancellationToken);

        exchange.FromDocument.Restore(_currentUser.UserId);
        exchange.ToDocument.Restore(_currentUser.UserId);
        exchange.Restore(_currentUser.UserId);

        await _ledgerValidator.ValidateAsync(fromFundSource, exchange.FromDocument.DocumentDate, 0, fromEntry.Credit,
            replacingEntryId: fromEntry.Id, cancellationToken);

        fromFundSource.AdjustBalance(-fromEntry.Credit);
        toFundSource.AdjustBalance(toEntry.Debit);


        await _attachmentService.RestoreAllForOwnerAsync(
            AttachmentOwnerType.CurrencyExchange, exchange.Id, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
