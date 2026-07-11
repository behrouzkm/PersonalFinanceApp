using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;

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
        var document = await _context.AccountingDocuments
            .Include(d => d.Entries)
            .FirstOrDefaultAsync(r => r.Id == request.AccountingDocumentId, cancellationToken)
            ?? throw new NotFoundException(nameof(AccountingDocument), request.AccountingDocumentId);

        _context.Entry(document).Property(d => d.RowVersion).OriginalValue = request.RowVersion;


        document.SetDocumentDate(request.DocumentDate);
        document.SetCurrencyId(request.CurrencyId);
        document.SetDescription(request.Description);


        // load every money/person account that could be touched
        var existingCreditEntries = document.Entries.Where(r => r.Credit > 0).ToList();
        var existingCreditLedgerAccountIds = existingCreditEntries.Select(s => s.LedgerAccountId).Distinct().ToList();

        var requestedMonetaryAccountIds = request.MonetaryAccountPayments.Select(s => s.MonetaryAccountId).Distinct().ToList();
        var requestedPersonIds = request.PersonPayments.Select(s => s.PersonId).Distinct().ToList();

        var monetaryAccounts = await _context.MonetaryAccounts
            .Where(r => requestedMonetaryAccountIds.Contains(r.Id) ||
                    existingCreditLedgerAccountIds.Contains(r.LedgerAccountId))
            .ToListAsync();

        var persons = await _context.Persons
            .Where(r => requestedPersonIds.Contains(r.Id) ||
                    existingCreditLedgerAccountIds.Contains(r.LedgerAccountId))
            .ToListAsync();


        var monetaryAccountsById = monetaryAccounts.ToDictionary(s => s.Id);
        var monetaryAccountsByLedgerId = monetaryAccounts.ToDictionary(s => s.LedgerAccountId);

        var personAccountById = persons.ToDictionary(d => d.Id);
        var personAccountByLedgerId = persons.ToDictionary(d => d.LedgerAccountId);

        var expenseAccountIds = request.ExpenditureLines.Select(s => s.ExpenseLedgerAccountId).Distinct().ToList();
        var expenseAccounts = await _context.LedgerAccounts
            .Where(r => expenseAccountIds.Contains(r.Id))
            .ToDictionaryAsync(d => d.Id, cancellationToken);


        var existingEntriesById = document.Entries.ToDictionary(d => d.Id);
        var modifiedBy = _currentUser.UserId;


        // -- Expense (debit) entries --
        SetExpenditureEntries(document, request, existingEntriesById, expenseAccounts, modifiedBy);

        // -- Monetary account (credit) entries --
        SetMonetaryAccountsPayment(document, request, existingEntriesById, monetaryAccountsById, monetaryAccountsByLedgerId, modifiedBy);

        // -- Person (credit) entries --
        SetPersonPayments(document, request, existingEntriesById, personAccountById, personAccountByLedgerId, modifiedBy);

        await _context.SaveChangesAsync(cancellationToken);
    }

    private void SetExpenditureEntries(AccountingDocument document,
                                        UpdateExpenditureCommand request,
                                        Dictionary<Guid, AccountingEntry> existingEntriesById,
                                        Dictionary<Guid, LedgerAccount> expenseAccounts,
                                        Guid modifiedBy)
    {

        var requestIds = request.ExpenditureLines
            .Where(r => r.AccountingEntryId.HasValue)
            .Select(s => s.AccountingEntryId!.Value)
            .ToHashSet();

        foreach (var oldEntry in document.Entries.Where(e => e.Debit > 0 && !requestIds.Contains(e.Id)).ToList())
        {
            document.RemoveEntry(oldEntry, modifiedBy);
        }

        foreach (var line in request.ExpenditureLines)
        {
            var account = expenseAccounts.TryGetValue(line.ExpenseLedgerAccountId, out var acc)
                ? acc : throw new NotFoundException(nameof(LedgerAccount), line.ExpenseLedgerAccountId);


            if (!account.IsPostingAccount)
                throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.ExpenseAccountNotPostable,
                                                    account.Id, account.Name);

            // new row
            if (line.AccountingEntryId is null)
            {
                document.AddEntry(line.ExpenseLedgerAccountId, line.Amount, 0, line.Description, modifiedBy);
                account.MarkAsUsed();
                continue;
            }

            // update existing row
            var entry = existingEntriesById.TryGetValue(line.AccountingEntryId.Value, out var existing)
                    ? existing
                    : throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.EntryNotFoundOnDocument,
                        line.AccountingEntryId.Value);

            var desciption = line.Description ?? string.Empty;
            var changed = entry.LedgerAccountId != line.ExpenseLedgerAccountId
                            || entry.Debit != line.Amount
                            || entry.Description != desciption;

            if (!changed)
                continue;

            entry.UpdateEntry(line.ExpenseLedgerAccountId, line.Amount, 0, desciption);
            entry.UpdateAudit(modifiedBy);

            account.MarkAsUsed();

        }

    }

    private void SetMonetaryAccountsPayment(AccountingDocument document,
                                                UpdateExpenditureCommand request,
                                                Dictionary<Guid, AccountingEntry> existingEntriesById,
                                                Dictionary<Guid, MonetaryAccount> monetaryAccountsById,
                                                Dictionary<Guid, MonetaryAccount> monetaryAccountsByLedgerId,
                                                Guid modifiedBy)
    {
        var requestedIds = request.MonetaryAccountPayments
            .Where(r => r.AccountingEntryId.HasValue)
            .Select(p => p.AccountingEntryId!.Value)
            .ToHashSet();

        // rows which user removed - reverse the withdrawal , then soft delete the entry
        foreach (var oldEntry in document.Entries.Where(r => r.Credit > 0 && !requestedIds.Contains(r.Id)).ToList())
        {
            if (monetaryAccountsByLedgerId.TryGetValue(oldEntry.LedgerAccountId, out var oldAccount))
            {
                oldAccount.AdjustBalance(oldEntry.Credit);
                document.RemoveEntry(oldEntry, modifiedBy);
            }
        }

        foreach (var payment in request.MonetaryAccountPayments)
        {
            var moneyAccount = monetaryAccountsById.TryGetValue(payment.MonetaryAccountId, out var mon)
                    ? mon
                    : throw new NotFoundException(nameof(MonetaryAccount), payment.MonetaryAccountId);



            if (payment.AccountingEntryId is null)
            {
                // new row
                document.EnsureCurrencyMatches(moneyAccount.CurrencyId);

                if (!moneyAccount.CanWithdraw(payment.Amount))
                    throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.InsufficientBalance,
                        moneyAccount.Id, moneyAccount.Name, payment.Amount);

                document.AddEntry(moneyAccount.LedgerAccountId, 0, payment.Amount, payment.Description, modifiedBy);
                moneyAccount.AdjustBalance(-payment.Amount);
                continue;
            }


            var entry = existingEntriesById.TryGetValue(payment.AccountingEntryId.Value, out var existing)
                ? existing
                : throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.EntryNotFoundOnDocument);

            var description = payment.Description ?? string.Empty;
            var ledgerAccountChanged = entry.LedgerAccountId != moneyAccount.LedgerAccountId;

            if (ledgerAccountChanged)
            {
                // reverse the old account balance
                if (monetaryAccountsByLedgerId.TryGetValue(entry.LedgerAccountId, out var oldAccount))
                {
                    oldAccount.AdjustBalance(entry.Credit);
                }

                document.EnsureCurrencyMatches(moneyAccount.CurrencyId);
                if (!moneyAccount.CanWithdraw(payment.Amount))
                    throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.InsufficientBalance,
                        moneyAccount.Id, moneyAccount.Name, payment.Amount);

                entry.UpdateEntry(moneyAccount.LedgerAccountId, 0, payment.Amount, description);
                entry.UpdateAudit(modifiedBy);
                moneyAccount.AdjustBalance(-payment.Amount);

                continue;
            }

            // same account, we only need to check if the amount and/or description has changed
            var amountDelta = payment.Amount - entry.Credit;
            if (amountDelta == 0 && entry.Description == description)
                continue;

            if (amountDelta > 0 && !moneyAccount.CanWithdraw(amountDelta))
                throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.InsufficientBalance,
                    moneyAccount.Id, moneyAccount.Name, amountDelta);

            entry.SetAmounts(0, payment.Amount);
            entry.SetDescription(description);
            entry.UpdateAudit(modifiedBy);
            moneyAccount.AdjustBalance(-amountDelta);

        }
    }

    private void SetPersonPayments(AccountingDocument document,
                                    UpdateExpenditureCommand request,
                                    Dictionary<Guid, AccountingEntry> existingEntriesById,
                                    Dictionary<Guid, Person> personAccountById,
                                    Dictionary<Guid, Person> personAccountByLedgerId,
                                    Guid modifiedBy)
    {
        var requestedIds = request.PersonPayments
            .Where(r => r.AccountingEntryId.HasValue)
            .Select(p => p.AccountingEntryId!.Value)
            .ToHashSet();

        // rows which user removed - reverse the withdrawal
        foreach (var oldEntry in document.Entries.Where(r => r.Credit > 0 && !requestedIds.Contains(r.Id)).ToList())
        {
            if (personAccountByLedgerId.TryGetValue(oldEntry.LedgerAccountId, out var oldPersonAccount))
            {
                // reverse the old person account balance
                oldPersonAccount.AdjustBalance(oldEntry.Credit);
                document.RemoveEntry(oldEntry, modifiedBy);
            }
        }

        foreach (var payment in request.PersonPayments)
        {
            var personAccount = personAccountById.TryGetValue(payment.PersonId, out var per)
                    ? per
                    : throw new NotFoundException(nameof(Person), payment.PersonId);


            if (payment.AccountingEntryId is null)
            {
                // new row
                document.EnsureCurrencyMatches(personAccount.CurrencyId);
                document.AddEntry(personAccount.LedgerAccountId, 0, payment.Amount, payment.Description, modifiedBy);
                personAccount.AdjustBalance(-payment.Amount);
                continue;
            }


            var entry = existingEntriesById.TryGetValue(payment.AccountingEntryId.Value, out var existing)
                ? existing
                : throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.EntryNotFoundOnDocument,
                                                     payment.AccountingEntryId.Value);

            var description = payment.Description ?? string.Empty;
            var ledgerAccountChanged = entry.LedgerAccountId != personAccount.LedgerAccountId;

            if (ledgerAccountChanged)
            {
                // reverse the old account balance
                if (personAccountByLedgerId.TryGetValue(entry.LedgerAccountId, out var oldPersonAccount))
                {
                    oldPersonAccount.AdjustBalance(entry.Credit);
                }

                document.EnsureCurrencyMatches(personAccount.CurrencyId);
                entry.UpdateEntry(personAccount.LedgerAccountId, 0, payment.Amount, description);
                entry.UpdateAudit(modifiedBy);
                personAccount.AdjustBalance(-payment.Amount);

                continue;
            }

            // same account, we only need to check if the amount and/or description has changed
            var amountDelta = payment.Amount - entry.Credit;
            if (amountDelta == 0 && entry.Description == description)
                continue;

            entry.SetAmounts(0, payment.Amount);
            entry.SetDescription(description);
            entry.UpdateAudit(modifiedBy);
            personAccount.AdjustBalance(-amountDelta);

        }
    }
}
