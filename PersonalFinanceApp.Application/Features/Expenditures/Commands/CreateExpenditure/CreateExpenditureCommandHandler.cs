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

namespace PersonalFinanceApp.Application.Features.Expenditures.Commands.CreateExpenditure;

public class CreateExpenditureCommandHandler : IRequestHandler<CreateExpenditureCommand, Guid>
{

    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateExpenditureCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateExpenditureCommand request, CancellationToken cancellationToken)
    {

        // load and validate every expense (debit) side account
        var expenseAccountIds = request.ExpenditureLines
            .Select(l => l.ExpenseLedgerAccountId)
            .Distinct()
            .ToList();

        var expenseAccounts = await _context.LedgerAccounts
            .Where(a => expenseAccountIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, cancellationToken);

        foreach (var id in expenseAccountIds)
        {
            if (!expenseAccounts.TryGetValue(id, out var account))
                throw new NotFoundException(nameof(LedgerAccount), id);

            if (!account.IsPostingAccount)
                throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.ExpenseAccountNotPostable,
                    account.Id, account.Name);
        }

        // --- Load every monetary account and person referenced on the payment side ---
        var monetaryAccountIds = request.MonetaryAccountPayments
            .Select(p => p.MonetaryAccountId)
            .Distinct()
            .ToList();

        var monetaryAccounts = await _context.MonetaryAccounts
            .Where(a => monetaryAccountIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, cancellationToken);

        var personIds = request.PersonPayments
            .Select(p => p.PersonId)
            .Distinct()
            .ToList();

        var persons = await _context.Persons
            .Where(p => personIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        foreach (var payment in request.MonetaryAccountPayments)
        {
            if (!monetaryAccounts.TryGetValue(payment.MonetaryAccountId, out var account))
                throw new NotFoundException(nameof(MonetaryAccount), payment.MonetaryAccountId);

            if (!account.CanWithdraw(payment.Amount))
                throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.InsufficientBalance,
                    account.Id, account.Name, payment.Amount);

        }

        foreach (var payment in request.PersonPayments)
        {
            if (!persons.ContainsKey(payment.PersonId))
                throw new NotFoundException(nameof(Person), payment.PersonId);

         }

        var expenditure = new AccountingDocument(
            DocumentType.Expenditure,
            request.DocumentDate,
            request.CurrencyId,
            _currentUser.TenantId,
            _currentUser.UserId);

        foreach (var line in request.ExpenditureLines)
        {
            expenditure.AddEntry(line.ExpenseLedgerAccountId, line.Amount, 0, line.Description ?? string.Empty);

            expenseAccounts[line.ExpenseLedgerAccountId].MarkAsUsed();
        }


        foreach (var payment in request.MonetaryAccountPayments)
        {
            var monetaryAccount = monetaryAccounts[payment.MonetaryAccountId];

            // enforce that the account's native currency matches this document's currency.
            expenditure.EnsureCurrencyMatches(monetaryAccount.CurrencyId);

            expenditure.AddEntry(monetaryAccount.LedgerAccountId, 0, payment.Amount, payment.Description ?? string.Empty);
            monetaryAccount.AdjustBalance(-payment.Amount);
        }

        foreach (var payment in request.PersonPayments)
        {
            var person = persons[payment.PersonId];

            // enforce that the person's native currency matches this document's currency.
            expenditure.EnsureCurrencyMatches(person.CurrencyId);

            expenditure.AddEntry(person.LedgerAccountId, 0, payment.Amount, payment.Description ?? string.Empty);
            person.AdjustBalance(-payment.Amount);
        }

        _context.AccountingDocuments.Add(expenditure);
        await _context.SaveChangesAsync(cancellationToken);

        return expenditure.Id;
    }
}
