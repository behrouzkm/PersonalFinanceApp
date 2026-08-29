using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Application.Features.Expenditures.Commands.UpdateExpenditure;

public class UpdateExpenditureCommandHandler : IRequestHandler<UpdateExpenditureCommand>
{

    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAccountingLookupService _lookupService;
    private readonly ILedgerBalanceValidationService _ledgerValidator;
    public UpdateExpenditureCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IAccountingLookupService lookupService,
        ILedgerBalanceValidationService ledgerValidator)
    {
        _context = context;
        _currentUser = currentUser;
        _lookupService = lookupService;
        _ledgerValidator = ledgerValidator;
    }

    public async Task Handle(UpdateExpenditureCommand request, CancellationToken cancellationToken)
    {
        // load the document to update, including all its entries
        var document = await _context.AccountingDocuments
            .Include(d => d.Entries)
            .FirstOrDefaultAsync(r => r.Id == request.AccountingDocumentId, cancellationToken)
            ?? throw new NotFoundException(nameof(AccountingDocument), request.AccountingDocumentId);

        _context.Entry(document).Property(d => d.RowVersion).OriginalValue = request.RowVersion;


        // update the document's header fields
        document.UpdateAccountingDocument(request.DocumentDate, request.CurrencyId, _currentUser.UserId, request.Description);


        // load every money/person account that could be touched
        var existingCreditEntries = document.Entries.Where(r => r.Credit > 0).ToList();
        var existingCreditLedgerAccountIds = existingCreditEntries.Select(s => s.LedgerAccountId).Distinct().ToList();

        var requestedMonetaryAccountIds = request.MonetaryAccountEntries.Select(s => s.MonetaryAccountId).Distinct().ToList();
        var requestedPersonIds = request.PersonPaymentEntries.Select(s => s.PersonId).Distinct().ToList();

        // load all monetary accounts and persons that are either in the request or already on the document
        var monetaryAccountLookup = await _lookupService.GetMonetaryAccountsAsync(
         requestedMonetaryAccountIds, existingCreditLedgerAccountIds, cancellationToken);

        var personLookup = await _lookupService.GetPersonsAsync(
            requestedPersonIds, existingCreditLedgerAccountIds, cancellationToken);

        var expenseAccountIds = request.ExpenditureLedgerAccountLines.Select(s => s.LedgerAccountId).Distinct().ToList();
        var expenseAccounts = await _lookupService.GetLedgerAccountsAsync(expenseAccountIds, cancellationToken);

        var existingEntriesById = document.Entries.ToDictionary(d => d.Id);
        var modifiedBy = _currentUser.UserId;


        // -- Expense (debit) entries --
        SetExpenditureEntries(document, request, existingEntriesById, expenseAccounts, modifiedBy);

        // -- Monetary account (credit) entries --
        await SetPaymentAsync<MonetaryAccount>(document, request.MonetaryAccountEntries, request.DocumentDate,
                existingEntriesById, monetaryAccountLookup.ById, monetaryAccountLookup.ByLedgerAccountId,
                nameof(MonetaryAccount), modifiedBy, cancellationToken);
        // -- Person (credit) entries --
        await SetPaymentAsync<Person>(document, request.PersonPaymentEntries, request.DocumentDate,
                existingEntriesById, personLookup.ById, personLookup.ByLedgerAccountId,
                nameof(Person), modifiedBy, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    private void SetExpenditureEntries(AccountingDocument document,
                                        UpdateExpenditureCommand request,
                                        Dictionary<Guid, AccountingEntry> existingEntriesById,
                                        Dictionary<Guid, LedgerAccount> expenseAccounts,
                                        Guid modifiedBy)
    {

        // get document's existing debit entries from request, so we can detect which ones were removed
        // these Ids are existing debit entries on the document, not new ones to be added or removed
        var requestIds = request.ExpenditureLedgerAccountLines
            .Where(r => r.AccountingEntryId.HasValue)
            .Select(s => s.AccountingEntryId!.Value)
            .ToHashSet();

        // debit rows which user removed : the Ids which is not available on the document - soft delete the entry
        foreach (var oldEntry in document.Entries.Where(e => e.Debit > 0 && !requestIds.Contains(e.Id)).ToList())
        {
            document.RemoveEntry(oldEntry, modifiedBy);
        }

        // process new debit (expenditure) rows or updated existing debit rows
        foreach (var line in request.ExpenditureLedgerAccountLines)
        {
            var expenseLedgerAccount = expenseAccounts.TryGetValue(line.LedgerAccountId, out var acc)
                ? acc
                : throw new NotFoundException(nameof(LedgerAccount), line.LedgerAccountId);


            if (!expenseLedgerAccount.IsPostingAccount)
                throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.ExpenseAccountNotPostable,
                    expenseLedgerAccount.Id, expenseLedgerAccount.Name);

            // new debit row
            if (line.AccountingEntryId is null)
            {
                document.AddEntry(line.LedgerAccountId, line.Amount, 0, line.Description, modifiedBy);
                expenseLedgerAccount.MarkAsUsed();
                continue;
            }

            // update existing debit row
            var existingExpenditureEntry = existingEntriesById.TryGetValue(line.AccountingEntryId.Value, out var existing)
                    ? existing
                    : throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.EntryNotFoundOnDocument,
                        line.AccountingEntryId.Value);

            var changed = existingExpenditureEntry.LedgerAccountId != line.LedgerAccountId
                            || existingExpenditureEntry.Debit != line.Amount
                            || existingExpenditureEntry.Description != line.Description;

            if (!changed)
                continue;

            existingExpenditureEntry.UpdateEntry(line.LedgerAccountId, line.Amount, 0, line.Description);
            existingExpenditureEntry.UpdateAudit(modifiedBy);

            expenseLedgerAccount.MarkAsUsed();

        }

    }

    private async Task SetPaymentAsync<TSource>(
        AccountingDocument document,
        IEnumerable<IPaymentDto> payments,
        DateOnly documentDate,
        Dictionary<Guid, AccountingEntry> existingEntriesById,
        Dictionary<Guid, TSource> fundSourceAccountsById,
        Dictionary<Guid, TSource> fundSourceAccountsByLedgerId,
        string entityName,
        Guid modifiedBy,
        CancellationToken cancellationToken)
        where TSource : class, IFundSource
    {
        // get document's existing credit (Monetary Account/Person payments) entries from request, so we can detect which ones were removed
        // these Ids are existing credit entries on the document, not new ones to be added or removed
        var requestedIds = payments
            .Where(r => r.AccountingEntryId.HasValue)
            .Select(p => p.AccountingEntryId!.Value)
            .ToHashSet();

        // payment rows which user removed - reverse the withdrawal , then soft delete the entry
        foreach (var oldEntry in document.Entries.Where(r => r.Credit > 0 && !requestedIds.Contains(r.Id)).ToList())
        {
            if (fundSourceAccountsByLedgerId.TryGetValue(oldEntry.LedgerAccountId, out var oldSourceAccount))
            {
                oldSourceAccount.AdjustBalance(oldEntry.Credit);
                document.RemoveEntry(oldEntry, modifiedBy);

                await _ledgerValidator.ValidateRemovalAsync(oldSourceAccount, oldEntry.Id, cancellationToken);
            }
        }

        // process new credit (Monetary Account/Person payments) rows or updated existing credit rows
        foreach (var payment in payments)
        {
            var paymentSource = fundSourceAccountsById.TryGetValue(payment.FundSourceId, out var src)
                    ? src
                    : throw new NotFoundException(entityName, payment.FundSourceId);

            var ledgerAccountEntity = GetLedgerAccount(paymentSource);


            // new credit (Monetary Account/Person payments) row
            if (payment.AccountingEntryId is null)
            {
                document.EnsureCurrencyMatches(paymentSource.CurrencyId);

                if (!paymentSource.CanWithdraw(payment.Amount))
                    throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.InsufficientBalance,
                        paymentSource.Id, paymentSource.DisplayName, payment.Amount);

                await _ledgerValidator.ValidateAsync(paymentSource, documentDate, 0, payment.Amount,
                    replacingEntryId: null, cancellationToken);

                document.AddEntry(paymentSource.LedgerAccountId, 0, payment.Amount, payment.Description, modifiedBy);
                ledgerAccountEntity.MarkAsUsed();
                paymentSource.AdjustBalance(-payment.Amount);
                continue;
            }


            var entry = existingEntriesById.TryGetValue(payment.AccountingEntryId.Value, out var existing)
                ? existing
                : throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.EntryNotFoundOnDocument);

            var ledgerAccountChanged = entry.LedgerAccountId != paymentSource.LedgerAccountId;

            // if the ledger account has changed, we need to reverse the old account's balance and apply
            // the new account's balance
            if (ledgerAccountChanged)
            {
                // reverse the old account balance
                if (fundSourceAccountsByLedgerId.TryGetValue(entry.LedgerAccountId, out var oldAccount))
                {
                    oldAccount.AdjustBalance(entry.Credit);
                    await _ledgerValidator.ValidateRemovalAsync(oldAccount, entry.Id, cancellationToken);
                }

                document.EnsureCurrencyMatches(paymentSource.CurrencyId);

                if (!paymentSource.CanWithdraw(payment.Amount))
                    throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.InsufficientBalance,
                        paymentSource.Id, paymentSource.DisplayName, payment.Amount);

                await _ledgerValidator.ValidateAsync(paymentSource, documentDate, 0, payment.Amount,
                        replacingEntryId: entry.Id, cancellationToken);

                entry.UpdateEntry(paymentSource.LedgerAccountId, 0, payment.Amount, payment.Description);
                entry.UpdateAudit(modifiedBy);
                paymentSource.AdjustBalance(-payment.Amount);

                ledgerAccountEntity.MarkAsUsed();

                continue;
            }

            // same account, we only need to check if the amount and/or description has changed
            var amountDelta = payment.Amount - entry.Credit;
            if (amountDelta == 0 && entry.Description == payment.Description)
                continue;

            if (amountDelta > 0 && !paymentSource.CanWithdraw(amountDelta))
                throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.InsufficientBalance,
                    paymentSource.Id, paymentSource.DisplayName, amountDelta);

            await _ledgerValidator.ValidateAsync(paymentSource, documentDate, 0, payment.Amount,
                      replacingEntryId: entry.Id, cancellationToken);

            entry.SetAmounts(0, payment.Amount);
            entry.SetDescription(payment.Description);
            entry.UpdateAudit(modifiedBy);
            paymentSource.AdjustBalance(-amountDelta);

        }

    }

    // IFundSource doesn't expose the LedgerAccount navigation (only LedgerAccountId),
    // so MarkAsUsed() needs the concrete entity's own property - this switch keeps that
    // one narrow cast in a single place instead of repeating it at every call site.
    private static LedgerAccount GetLedgerAccount(IFundSource source) => source switch
    {
        MonetaryAccount monetaryAccount => monetaryAccount.LedgerAccount,
        Person person => person.LedgerAccount,
        _ => throw new NotSupportedException($"Unsupported fund source type: {source.GetType().Name}")
    };
}
