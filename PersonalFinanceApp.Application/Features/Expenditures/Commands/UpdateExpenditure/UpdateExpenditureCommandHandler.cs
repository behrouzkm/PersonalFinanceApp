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

    public UpdateExpenditureCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
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
        document.SetDocumentDate(request.DocumentDate);
        document.SetCurrencyId(request.CurrencyId);
        document.SetDescription(request.Description);


        // load every money/person account that could be touched
        var existingCreditEntries = document.Entries.Where(r => r.Credit > 0).ToList();
        var existingCreditLedgerAccountIds = existingCreditEntries.Select(s => s.LedgerAccountId).Distinct().ToList();

        var requestedMonetaryAccountIds = request.MonetaryAccountEntries.Select(s => s.MonetaryLedgerAccountId).Distinct().ToList();
        var requestedPersonIds = request.PersonPaymentEntries.Select(s => s.PersonId).Distinct().ToList();

        // load all monetary accounts and persons that are either in the request or already on the document
        var monetaryAccounts = await _context.MonetaryAccounts
            .Include(m => m.LedgerAccount)
            .Where(r => requestedMonetaryAccountIds.Contains(r.Id) ||
                    existingCreditLedgerAccountIds.Contains(r.LedgerAccountId))
            .ToListAsync();

        var persons = await _context.Persons
            .Where(r => requestedPersonIds.Contains(r.Id) ||
                    existingCreditLedgerAccountIds.Contains(r.LedgerAccountId))
            .ToListAsync();


        var monetaryAccountsById = monetaryAccounts.ToDictionary(s => s.Id);
        var monetaryAccountsByLedgerId = monetaryAccounts.ToDictionary(s => s.LedgerAccountId);
        var monetaryLedgerAccounts = monetaryAccounts.ToDictionary(s => s.LedgerAccountId, s => s.LedgerAccount);

        var personAccountById = persons.ToDictionary(d => d.Id);
        var personAccountByLedgerId = persons.ToDictionary(d => d.LedgerAccountId);
        var personLedgerAccounts = persons.ToDictionary(d => d.LedgerAccountId, d => d.LedgerAccount);

        var expenseAccountIds = request.ExpenditureLedgerAccountLines.Select(s => s.LedgerAccountId).Distinct().ToList();
        var expenseAccounts = await _context.LedgerAccounts
            .Where(r => expenseAccountIds.Contains(r.Id))
            .ToDictionaryAsync(d => d.Id, cancellationToken);


        var existingEntriesById = document.Entries.ToDictionary(d => d.Id);
        var modifiedBy = _currentUser.UserId;


        // -- Expense (debit) entries --
        SetExpenditureEntries(document, request, existingEntriesById, expenseAccounts, modifiedBy);

        // -- Monetary account (credit) entries --
        SetPayment<MonetaryAccount>(document, request.MonetaryAccountEntries, existingEntriesById, monetaryAccountsById,
                    monetaryAccountsByLedgerId, monetaryLedgerAccounts, nameof(MonetaryAccount), modifiedBy);
        // -- Person (credit) entries --
        SetPayment<Person>(document, request.PersonPaymentEntries, existingEntriesById, personAccountById,
                    personAccountByLedgerId, personLedgerAccounts, nameof(Person), modifiedBy);

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
                ? acc : throw new NotFoundException(nameof(LedgerAccount), line.LedgerAccountId);


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

    private void SetPayment<TSource>(AccountingDocument document,
                                IEnumerable<IPaymentDto> payments,
                                Dictionary<Guid, AccountingEntry> existingEntriesById,
                                Dictionary<Guid, TSource> fundSourceAccountsById,
                                Dictionary<Guid, TSource> fundSourceAccountsByLedgerId,
                                Dictionary<Guid, LedgerAccount> fundSourceLedgerAccounts,
                                string entityName,
                                Guid modifiedBy)
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
            }
        }

        // process new credit (Monetary Account/Person payments) rows or updated existing credit rows
        foreach (var payment in payments)
        {
            var paymentLedgerAccount = fundSourceAccountsById.TryGetValue(payment.FundSourceId, out var src)
                    ? src
                    : throw new NotFoundException(entityName, payment.FundSourceId);

            var ledgerAccount = fundSourceLedgerAccounts[paymentLedgerAccount.LedgerAccountId];


            // new credit (Monetary Account/Person payments) row
            if (payment.AccountingEntryId is null)
            {
                document.EnsureCurrencyMatches(paymentLedgerAccount.CurrencyId);
                if (!paymentLedgerAccount.CanWithdraw(payment.Amount))
                    throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.InsufficientBalance,
                        paymentLedgerAccount.Id, paymentLedgerAccount.DisplayName, payment.Amount);

                document.AddEntry(paymentLedgerAccount.LedgerAccountId, 0, payment.Amount, payment.Description, modifiedBy);
                ledgerAccount.MarkAsUsed();
                paymentLedgerAccount.AdjustBalance(-payment.Amount);
                continue;
            }


            var entry = existingEntriesById.TryGetValue(payment.AccountingEntryId.Value, out var existing)
                ? existing
                : throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.EntryNotFoundOnDocument);

            var ledgerAccountChanged = entry.LedgerAccountId != paymentLedgerAccount.LedgerAccountId;

            // if the ledger account has changed, we need to reverse the old account's balance and apply
            // the new account's balance
            if (ledgerAccountChanged)
            {
                // reverse the old account balance
                if (fundSourceAccountsByLedgerId.TryGetValue(entry.LedgerAccountId, out var oldAccount))
                {
                    oldAccount.AdjustBalance(entry.Credit);
                }

                document.EnsureCurrencyMatches(paymentLedgerAccount.CurrencyId);

                if (!paymentLedgerAccount.CanWithdraw(payment.Amount))
                    throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.InsufficientBalance,
                        paymentLedgerAccount.Id, paymentLedgerAccount.DisplayName, payment.Amount);

                entry.UpdateEntry(paymentLedgerAccount.LedgerAccountId, 0, payment.Amount, payment.Description);
                entry.UpdateAudit(modifiedBy);
                paymentLedgerAccount.AdjustBalance(-payment.Amount);

                ledgerAccount.MarkAsUsed();

                continue;
            }

            // same account, we only need to check if the amount and/or description has changed
            var amountDelta = payment.Amount - entry.Credit;
            if (amountDelta == 0 && entry.Description == payment.Description)
                continue;

            if (amountDelta > 0 && !paymentLedgerAccount.CanWithdraw(amountDelta))
                throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.InsufficientBalance,
                    paymentLedgerAccount.Id, paymentLedgerAccount.DisplayName, amountDelta);

            entry.SetAmounts(0, payment.Amount);
            entry.SetDescription(payment.Description);
            entry.UpdateAudit(modifiedBy);
            paymentLedgerAccount.AdjustBalance(-amountDelta);

        }
    }
}
