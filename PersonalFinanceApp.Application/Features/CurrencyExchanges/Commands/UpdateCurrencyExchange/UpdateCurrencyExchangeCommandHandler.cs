using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Application.Features.CurrencyExchanges.Commands.UpdateCurrencyExchange;

public class UpdateCurrencyExchangeCommandHandler : IRequestHandler<UpdateCurrencyExchangeCommand>
{

    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAccountingLookupService _lookupService;
    private readonly ILedgerBalanceValidationService _ledgerValidator;

    public UpdateCurrencyExchangeCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser,
        IAccountingLookupService lookupService, ILedgerBalanceValidationService ledgerValidator)
    {
        _context = context;
        _currentUser = currentUser;
        _lookupService = lookupService;
        _ledgerValidator = ledgerValidator;
    }

    public async Task Handle(UpdateCurrencyExchangeCommand request, CancellationToken cancellationToken)
    {
        var exchange = await _context.CurrencyExchanges
            .Include(e => e.FromDocument).ThenInclude(d => d.Entries)
            .Include(e => e.ToDocument).ThenInclude(d => d.Entries)
            .FirstOrDefaultAsync(r => r.Id == request.CurrencyExchangeId, cancellationToken)
            ?? throw new NotFoundException(nameof(CurrencyExchange), request.CurrencyExchangeId);

        _context.Entry(exchange).Property(e => e.RowVersion).OriginalValue = request.RowVersion;

        var (newFromFundSource, newFromLedgerAccount) = await _lookupService
                   .GetFundSourceByLedgerAccountIdAsync(request.FromLedgerAccountId, cancellationToken);
        var (newToFundSource, newToLedgerAccount) = await _lookupService
            .GetFundSourceByLedgerAccountIdAsync(request.ToLedgerAccountId, cancellationToken);

        if (newFromFundSource.CurrencyId == newToFundSource.CurrencyId)
            throw new BusinessRuleException(ApplicationErrorCodes.CurrencyExchange.SameCurrencyExchangeNotAllowed);



        await UpdateSideAsync(
              exchange.FromDocument, request.FromLedgerAccountId, request.FromAmount, request.ExchangeDate,
              request.Description, newFromFundSource, newFromLedgerAccount, isFromSide: true, cancellationToken);

        await UpdateSideAsync(
            exchange.ToDocument, request.ToLedgerAccountId, request.ToAmount, request.ExchangeDate,
            request.Description, newToFundSource, newToLedgerAccount, isFromSide: false, cancellationToken);

        exchange.UpdateExchangeRate(request.ExchangeRate, _currentUser.UserId, request.Description);

        exchange.FromDocument.EnsureBalanced();
        exchange.ToDocument.EnsureBalanced();

        await _context.SaveChangesAsync(cancellationToken);

    }

    // isFromSide controls entry direction (fund source Credit vs Debit) and which
    // side of ILedgerBalanceValidationService applies (only the FROM side risks
    // violating a credit-limit floor — receiving money never does).
    private async Task UpdateSideAsync(
        AccountingDocument document, Guid newLedgerAccountId, decimal newAmount, DateOnly newDate,
        string? description, IFundSource newFundSource, LedgerAccount newLedgerAccount, bool isFromSide,
        CancellationToken cancellationToken)
    {
        var oldClearing = await _lookupService
            .GetOrCreateCurrencyExchangeClearingLedgerAccountAsync(document.CurrencyId, cancellationToken);

        var fundSourceEntry = document.Entries.FirstOrDefault(e => e.LedgerAccountId != oldClearing.Id)
            ?? throw new BusinessRuleException(ApplicationErrorCodes.CurrencyExchange.FundSourceEntryNotFound);

        var clearingEntry = document.Entries.FirstOrDefault(e => e.LedgerAccountId == oldClearing.Id)
            ?? throw new BusinessRuleException(ApplicationErrorCodes.CurrencyExchange.ClearingEntryNotFound);

        var oldAmount = isFromSide ? fundSourceEntry.Credit : fundSourceEntry.Debit;
        var fundSourceChanged = fundSourceEntry.LedgerAccountId != newLedgerAccountId;
        var newClearing = await _lookupService
            .GetOrCreateCurrencyExchangeClearingLedgerAccountAsync(newFundSource.CurrencyId, cancellationToken);
        var currencyChanged = newClearing.Id != oldClearing.Id;

        if (fundSourceChanged)
        {
            var (oldFundSource, _) = await _lookupService
                .GetFundSourceByLedgerAccountIdAsync(fundSourceEntry.LedgerAccountId, cancellationToken);

            if (isFromSide)
            {
                if (!newFundSource.CanWithdraw(newAmount))
                    throw new BusinessRuleException(ApplicationErrorCodes.CurrencyExchange.InsufficientBalance,
                        newFundSource.Id, newAmount);

                await _ledgerValidator.ValidateAsync(newFundSource, newDate, 0, newAmount,
                    replacingEntryId: null, cancellationToken);

                oldFundSource.AdjustBalance(oldAmount);       // reverse: give back what was withdrawn
                newFundSource.AdjustBalance(-newAmount);
            }
            else
            {
                await _ledgerValidator.ValidateRemovalAsync(oldFundSource, fundSourceEntry.Id, cancellationToken);

                oldFundSource.AdjustBalance(-oldAmount);       // reverse: take back what was received
                newFundSource.AdjustBalance(newAmount);
            }
        }
        else if (newAmount != oldAmount)
        {
            var amountDelta = newAmount - oldAmount;

            if (isFromSide && amountDelta > 0)
            {
                if (!newFundSource.CanWithdraw(amountDelta))
                    throw new BusinessRuleException(ApplicationErrorCodes.CurrencyExchange.InsufficientBalance,
                        newFundSource.Id, amountDelta);

                await _ledgerValidator.ValidateAsync(newFundSource, newDate, 0, newAmount,
                    replacingEntryId: fundSourceEntry.Id, cancellationToken);
            }
            else if (!isFromSide && amountDelta < 0)
            {
                await _ledgerValidator.ValidateAsync(newFundSource, newDate, newAmount, 0,
                    replacingEntryId: fundSourceEntry.Id, cancellationToken);
            }

            newFundSource.AdjustBalance(isFromSide ? -amountDelta : amountDelta);
        }

        if (currencyChanged)
        {
            document.UpdateAccountingDocument(newDate, newFundSource.CurrencyId, _currentUser.UserId, description);
            clearingEntry.UpdateEntry(newClearing.Id,
                isFromSide ? newAmount : 0, isFromSide ? 0 : newAmount, _currentUser.UserId, description);
            newClearing.MarkAsUsed();
        }
        else if (newDate != document.DocumentDate || description != document.Description)
        {
            document.UpdateAccountingDocument(newDate, document.CurrencyId, _currentUser.UserId, description);
            clearingEntry.UpdateEntry(
                isFromSide ? newAmount : 0, isFromSide ? 0 : newAmount, _currentUser.UserId, description);
        }
        else
        {
            clearingEntry.UpdateEntry(isFromSide ? newAmount : 0, isFromSide ? 0 : newAmount, _currentUser.UserId, description);
        }


        fundSourceEntry.UpdateEntry(newLedgerAccountId,
                isFromSide ? 0 : newAmount, isFromSide ? newAmount : 0, _currentUser.UserId, description);

        newLedgerAccount.MarkAsUsed();

    }
}
