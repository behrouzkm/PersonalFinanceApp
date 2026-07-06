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

    public UpdateExpenditureCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateExpenditureCommand request, CancellationToken cancellationToken)
    {
        var document = await _context.AccountingDocuments
            .Include(d => d.Entries)
            .FirstOrDefaultAsync(r => r.Id == request.AccountingDocumentId, cancellationToken)
            ?? throw new NotFoundException(nameof(AccountingDocument), request.AccountingDocumentId);

        _context.Entry(document).Property(d => d.RowVersion).OriginalValue = request.RowVersion;

        var oldPaymentLedgerAccountIds = document.Entries
            .Where(e => e.Credit > 0)
            .Select(s => s.LedgerAccountId)
            .Distinct()
            .ToList();

        var newMonetaryAccountsIds = request.MonetaryAccountPayments
            .Select(p => p.MonetaryAccountId)
            .Distinct()
            .ToList();

        var newPersonIds = request.PersonPayments
            .Select(s => s.PersonId)
            .Distinct()
            .ToList();

        var monetaryAccounts = await _context.MonetaryAccounts
            .Where(m => newMonetaryAccountsIds.Contains(m.Id) ||
                    oldPaymentLedgerAccountIds.Contains(m.LedgerAccountId))
            .ToListAsync(cancellationToken);

        var persons = await _context.Persons
            .Where(p => newPersonIds.Contains(p.Id) ||
                    oldPaymentLedgerAccountIds.Contains(p.LedgerAccountId))
            .ToListAsync(cancellationToken);


        var monetaryAccountsByLedgerId = monetaryAccounts.ToDictionary(d => d.LedgerAccountId);
        var monetaryAccountsById = monetaryAccounts.ToDictionary(d => d.Id);

        var personsByLedgerId = persons.ToDictionary(p => p.LedgerAccountId);
        var personsById = persons.ToDictionary(p => p.Id);

        // reversing the balance of old payment entries
        foreach (var oldEntry in document.Entries.Where(e => e.Credit > 0))
        {
            if (monetaryAccountsByLedgerId.TryGetValue(oldEntry.LedgerAccountId, out var moneyAccount))
            {
                moneyAccount.AdjustBalance(oldEntry.Credit);
            }
            else if (personsByLedgerId.TryGetValue(oldEntry.LedgerAccountId, out var person))
            {
                person.AdjustBalance(oldEntry.Credit);
            }
        }

        // load and validate the expense (debit) side accounts
        var expenseAccountIds = request.ExpenditureLines
            .Select(s => s.ExpenseLedgerAccountId)
            .Distinct()
            .ToList();

        var expenseAccounts = await _context.LedgerAccounts
            .Where(r => expenseAccountIds.Contains(r.Id))
            .ToDictionaryAsync(d => d.Id, cancellationToken);

        foreach (var id in expenseAccountIds)
        {
            if (!expenseAccounts.TryGetValue(id, out var ledgerAccount))
                throw new NotFoundException(nameof(LedgerAccount), id);

            if (!ledgerAccount.IsPostingAccount)
                throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.ExpenseAccountNotPostable,
                                                ledgerAccount.Id, ledgerAccount.Name);
        }

        // validate every new payment reference
        foreach (var payment in request.MonetaryAccountPayments)
        {
            if (!monetaryAccountsById.TryGetValue(payment.MonetaryAccountId, out var moneyAccount))
                throw new NotFoundException(nameof(MonetaryAccount), payment.MonetaryAccountId);

            if (!moneyAccount.CanWithdraw(payment.Amount))
                throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.InsufficientBalance,
                                                moneyAccount.Id, moneyAccount.Name, payment.Amount);
        }

        foreach (var payment in request.PersonPayments)
        {
            if (!personsById.ContainsKey(payment.PersonId))
                throw new NotFoundException(nameof(Person), payment.PersonId);
        }

        // replacing the document's entries and header fields
        foreach (var oldEntry in document.Entries.ToList())
        {
            document.RemoveEntry(oldEntry);
        }

        document.SetDocumentDate(request.DocumentDate);
        document.SetCurrencyId(request.CurrencyId);
        document.SetDescription(request.Description);

        foreach(var line in request.ExpenditureLines)
        {
            document.AddEntry(line.ExpenseLedgerAccountId,line.Amount,0,line.Description ?? string.Empty);
            expenseAccounts[line.ExpenseLedgerAccountId].MarkAsUsed();
        }

        foreach (var payment in request.MonetaryAccountPayments)
        {
            var moneyAccount = monetaryAccountsById[payment.MonetaryAccountId];

            document.EnsureCurrencyMatches(moneyAccount.CurrencyId);
            moneyAccount.AdjustBalance(-payment.Amount);
        }

        foreach (var payment in request.PersonPayments)
        {
            var person = personsById[payment.PersonId];

            document.EnsureCurrencyMatches(person.CurrencyId);
            person.AdjustBalance(-payment.Amount);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
