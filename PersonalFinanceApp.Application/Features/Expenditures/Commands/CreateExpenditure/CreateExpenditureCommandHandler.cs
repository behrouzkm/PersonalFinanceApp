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
        var expenseLedgerAccountIds = request.ExpenditureLedgerAccountLines
            .Select(l => l.LedgerAccountId)
            .Distinct()
            .ToList();

        var expenseLedgerAccounts = await _context.LedgerAccounts
            .Where(a => expenseLedgerAccountIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, cancellationToken);

        foreach (var id in expenseLedgerAccountIds)
        {
            if (!expenseLedgerAccounts.TryGetValue(id, out var account))
                throw new NotFoundException(nameof(LedgerAccount), id);

            if (!account.IsPostingAccount)
                throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.ExpenseAccountNotPostable,
                    account.Id, account.Name);
        }

        // --- Load every monetary account and person referenced on the payment side ---
        var monetaryAccountIds = request.MonetaryAccountEntries
            .Select(p => p.MonetaryLedgerAccountId)
            .Distinct()
            .ToList();

        var monetaryAccounts = await _context.MonetaryAccounts
            .Include(i => i.LedgerAccount)
            .Where(a => monetaryAccountIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, cancellationToken);

        var monetaryAccountsLedgerAccountIds = monetaryAccounts.Values
            .ToDictionary(ma => ma.LedgerAccountId, ma => ma.LedgerAccount);



        var personIds = request.PersonPaymentEntries
            .Select(p => p.PersonId)
            .Distinct()
            .ToList();

        var persons = await _context.Persons
            .Include(p => p.LedgerAccount)
            .Where(p => personIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        var personLedgerAccountIds = persons.Values
            .ToDictionary(p => p.LedgerAccountId, p => p.LedgerAccount);

        // check to missing references and insufficient funds before touching the document
        foreach (var payment in request.MonetaryAccountEntries)
        {
            if (!monetaryAccounts.TryGetValue(payment.MonetaryLedgerAccountId, out var account))
                throw new NotFoundException(nameof(MonetaryAccount), payment.MonetaryLedgerAccountId);

        }

        foreach (var payment in request.PersonPaymentEntries)
        {
            if (!persons.TryGetValue(payment.PersonId, out var person))
                throw new NotFoundException(nameof(Person), payment.PersonId);

        }


        // -- build the document --
        var expenditureDocument = new AccountingDocument(
            DocumentType.Expenditure,
            request.DocumentDate,
            request.CurrencyId,
            _currentUser.TenantId,
            _currentUser.UserId,
            request.Description);

        // add the expense (debit) side entries
        foreach (var line in request.ExpenditureLedgerAccountLines)
        {
            expenditureDocument.AddEntry(line.LedgerAccountId, line.Amount, 0, line.Description, _currentUser.UserId);

            expenseLedgerAccounts[line.LedgerAccountId].MarkAsUsed();
        }

        // add the payment (credit) side entries for monetary accounts
        foreach (var payment in request.MonetaryAccountEntries)
        {
            var monetaryAccount = monetaryAccounts[payment.MonetaryLedgerAccountId];
            var paymentLedgerAccount = monetaryAccountsLedgerAccountIds[monetaryAccount.LedgerAccountId];

            ApplyPayment(expenditureDocument, monetaryAccount, paymentLedgerAccount, payment.Amount, payment.Description, _currentUser.UserId);

        }

        // add the payment (credit) side entries for persons
        foreach (var payment in request.PersonPaymentEntries)
        {
            var person = persons[payment.PersonId];
            var paymentLedgerAccount = personLedgerAccountIds[person.LedgerAccountId];

            ApplyPayment(expenditureDocument, person, paymentLedgerAccount, payment.Amount, payment.Description, _currentUser.UserId);
        }

        _context.AccountingDocuments.Add(expenditureDocument);
        await _context.SaveChangesAsync(cancellationToken);

        return expenditureDocument.Id;
    }

    public void ApplyPayment(AccountingDocument accountingDocument, IFundSource source, LedgerAccount paymentLedgerAccount, decimal amount,
                                string? description, Guid actingUserId)
    {
        // enforce that the bank/person account's native currency matches this document's currency.
        accountingDocument.EnsureCurrencyMatches(source.CurrencyId);

        if (!source.CanWithdraw(amount))
            throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.InsufficientBalance,
                                                source.LedgerAccountId, amount);

        accountingDocument.AddEntry(source.LedgerAccountId, 0, amount, description, actingUserId);
        source.AdjustBalance(-amount);

        paymentLedgerAccount.MarkAsUsed();
    }

}
