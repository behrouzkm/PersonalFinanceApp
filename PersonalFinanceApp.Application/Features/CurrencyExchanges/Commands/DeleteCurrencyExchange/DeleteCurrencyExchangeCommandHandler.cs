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

namespace PersonalFinanceApp.Application.Features.CurrencyExchanges.Commands.DeleteCurrencyExchange;

public class DeleteCurrencyExchangeCommandHandler : IRequestHandler<DeleteCurrencyExchangeCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAccountingLookupService _lookupService;
    private readonly ILedgerBalanceValidationService _ledgerValidator;
    private readonly IAttachmentService _attachmentService;

    public DeleteCurrencyExchangeCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IAccountingLookupService lookupService,
        ILedgerBalanceValidationService ledgerValidator,
        IAttachmentService attachmentService)
    {
        _context = context;
        _currentUser = currentUser;
        _lookupService = lookupService;
        _ledgerValidator = ledgerValidator;
        _attachmentService = attachmentService;
    }

    public async Task Handle(DeleteCurrencyExchangeCommand request, CancellationToken cancellationToken)
    {
        var exchange = await _context.CurrencyExchanges
            .Include(e => e.FromDocument).ThenInclude(d => d.Entries)
            .Include(e => e.ToDocument).ThenInclude(d => d.Entries)
            .FirstOrDefaultAsync(e => e.Id == request.CurrencyExchangeId, cancellationToken)
            ?? throw new NotFoundException(nameof(CurrencyExchange), request.CurrencyExchangeId);

        _context.Entry(exchange).Property(e => e.RowVersion).OriginalValue = request.RowVersion;

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

        // Reversing the FROM side only ever adds money back — no floor to violate.
        // Reversing the TO side removes money that may since have been spent — validate.
        await _ledgerValidator.ValidateRemovalAsync(toFundSource, toEntry.Id, cancellationToken);

        fromFundSource.AdjustBalance(fromEntry.Credit);
        toFundSource.AdjustBalance(-toEntry.Debit);

        exchange.FromDocument.SoftDelete(_currentUser.UserId);
        exchange.ToDocument.SoftDelete(_currentUser.UserId);
        exchange.SoftDelete(_currentUser.UserId);

        await _attachmentService.SoftDeleteAllForOwnerAsync(
           AttachmentOwnerType.CurrencyExchange, exchange.Id, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
