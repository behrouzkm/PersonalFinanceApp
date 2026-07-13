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
        SetPayment(document, request.MonetaryAccountPayments, existingEntriesById, monetaryAccountsById,
                    monetaryAccountsByLedgerId,nameof(MonetaryAccount), modifiedBy);
        // -- Person (credit) entries --
        SetPayment(document, request.PersonPayments, existingEntriesById, personAccountById,
                    personAccountByLedgerId,nameof(Person), modifiedBy);

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

            var changed = entry.LedgerAccountId != line.ExpenseLedgerAccountId
                            || entry.Debit != line.Amount
                            || entry.Description != line.Description;

            if (!changed)
                continue;

            entry.UpdateEntry(line.ExpenseLedgerAccountId, line.Amount, 0, line.Description);
            entry.UpdateAudit(modifiedBy);

            account.MarkAsUsed();

        }

    }

    private void SetPayment<TSource>(AccountingDocument document,
                                IEnumerable<IPaymentDto> payments,
                                Dictionary<Guid, AccountingEntry> existingEntriesById,
                                Dictionary<Guid, TSource> fundSourceAccountsById,
                                Dictionary<Guid, TSource> fundSourceAccountsByLedgerId,
                                string entityName,
                                Guid modifiedBy)
                            where TSource : class, IFundSource
    {
        var requestedIds = payments
            .Where(r => r.AccountingEntryId.HasValue)
            .Select(p => p.AccountingEntryId!.Value)
            .ToHashSet();

        // rows which user removed - reverse the withdrawal , then soft delete the entry
        foreach (var oldEntry in document.Entries.Where(r => r.Credit > 0 && !requestedIds.Contains(r.Id)).ToList())
        {
            if (fundSourceAccountsByLedgerId.TryGetValue(oldEntry.LedgerAccountId, out var oldSourceAccount))
            {
                oldSourceAccount.AdjustBalance(oldEntry.Credit);
                document.RemoveEntry(oldEntry, modifiedBy);
            }
        }

        foreach (var payment in payments)
        {
            var source = fundSourceAccountsById.TryGetValue(payment.FundSourceId, out var src)
                    ? src
                    : throw new NotFoundException(entityName, payment.FundSourceId);



            if (payment.AccountingEntryId is null)
            {
                // new row
                document.EnsureCurrencyMatches(source.CurrencyId);

                if (!source.CanWithdraw(payment.Amount))
                    throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.InsufficientBalance,
                        source.Id, source.DisplayName, payment.Amount);

                document.AddEntry(source.LedgerAccountId, 0, payment.Amount, payment.Description, modifiedBy);
                source.AdjustBalance(-payment.Amount);
                continue;
            }


            var entry = existingEntriesById.TryGetValue(payment.AccountingEntryId.Value, out var existing)
                ? existing
                : throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.EntryNotFoundOnDocument);

            var ledgerAccountChanged = entry.LedgerAccountId != source.LedgerAccountId;

            if (ledgerAccountChanged)
            {
                // reverse the old account balance
                if (fundSourceAccountsByLedgerId.TryGetValue(entry.LedgerAccountId, out var oldAccount))
                {
                    oldAccount.AdjustBalance(entry.Credit);
                }

                document.EnsureCurrencyMatches(source.CurrencyId);
                if (!source.CanWithdraw(payment.Amount))
                    throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.InsufficientBalance,
                        source.Id, source.DisplayName, payment.Amount);

                entry.UpdateEntry(source.LedgerAccountId, 0, payment.Amount, payment.Description);
                entry.UpdateAudit(modifiedBy);
                source.AdjustBalance(-payment.Amount);

                continue;
            }

            // same account, we only need to check if the amount and/or description has changed
            var amountDelta = payment.Amount - entry.Credit;
            if (amountDelta == 0 && entry.Description == payment.Description)
                continue;

            if (amountDelta > 0 && !source.CanWithdraw(amountDelta))
                throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.InsufficientBalance,
                    source.Id, source.DisplayName, amountDelta);

            entry.SetAmounts(0, payment.Amount);
            entry.SetDescription(payment.Description);
            entry.UpdateAudit(modifiedBy);
            source.AdjustBalance(-amountDelta);

        }
    }
}
