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
    private readonly IAccountingLookupService _lookupService;
    private readonly ILedgerBalanceValidationService _ledgerValidator;

    public CreateExpenditureCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser,
                        IAccountingLookupService lookupService, ILedgerBalanceValidationService ledgerValidator)
    {
        _context = context;
        _currentUser = currentUser;
        _lookupService = lookupService;
        _ledgerValidator = ledgerValidator;
    }

    public async Task<Guid> Handle(CreateExpenditureCommand request, CancellationToken cancellationToken)
    {

        // load and validate every expense (debit) side account
        var expenseLedgerAccountIds = request.ExpenditureLedgerAccountLines
            .Select(l => l.LedgerAccountId)
            .Distinct()
            .ToList();

        var expenseLedgerAccounts = await _lookupService.GetLedgerAccountsAsync(expenseLedgerAccountIds, cancellationToken);

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
            .Select(p => p.MonetaryAccountId)
            .Distinct()
            .ToList();

        var monetaryAccountLookup = await _lookupService.GetMonetaryAccountsAsync(monetaryAccountIds,
                                                Enumerable.Empty<Guid>(), cancellationToken);


        var personIds = request.PersonPaymentEntries
            .Select(p => p.PersonId)
            .Distinct()
            .ToList();

        var personLookup = await _lookupService.GetPersonsAsync(personIds, Enumerable.Empty<Guid>(), cancellationToken);

        // check to missing references and insufficient funds before touching the document
        foreach (var payment in request.MonetaryAccountEntries)
        {
            if (!monetaryAccountLookup.ById.ContainsKey(payment.MonetaryAccountId))
                throw new NotFoundException(nameof(MonetaryAccount), payment.MonetaryAccountId);

        }

        foreach (var payment in request.PersonPaymentEntries)
        {
            if (!personLookup.ById.ContainsKey(payment.PersonId))
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
            var monetaryAccount = monetaryAccountLookup.ById[payment.MonetaryAccountId];

            ApplyPayment(expenditureDocument, monetaryAccount, monetaryAccount.LedgerAccount, request.DocumentDate,
                    payment.Amount, payment.Description, _currentUser.UserId, cancellationToken);

        }

        // add the payment (credit) side entries for persons
        foreach (var payment in request.PersonPaymentEntries)
        {
            var person = personLookup.ById[payment.PersonId];

            ApplyPayment(expenditureDocument, person, person.LedgerAccount, request.DocumentDate,
                    payment.Amount, payment.Description, _currentUser.UserId, cancellationToken);
        }

        _context.AccountingDocuments.Add(expenditureDocument);
        await _context.SaveChangesAsync(cancellationToken);

        return expenditureDocument.Id;
    }

    public async Task ApplyPayment(
        AccountingDocument accountingDocument,
        IFundSource source,
        LedgerAccount paymentLedgerAccount,
        DateOnly documentDate,
        decimal amount,
        string? description,
        Guid actingUserId,
        CancellationToken cancellationToken)
    {
        // enforce that the bank/person account's native currency matches this document's currency.
        accountingDocument.EnsureCurrencyMatches(source.CurrencyId);

        if (!source.CanWithdraw(amount))
            throw new BusinessRuleException(ApplicationErrorCodes.Expenditure.InsufficientBalance,
                                                source.LedgerAccountId, amount);


        // Authoritative check: replays the account's full chronological history (not just
        // the current in-memory balance CanWithdraw looked at above) and rejects if the
        // running balance would ever dip below the credit limit at any intermediate point.
        await _ledgerValidator.ValidateAsync(source, documentDate, 0, amount, replacingEntryId: null, cancellationToken);

        accountingDocument.AddEntry(source.LedgerAccountId, 0, amount, description, actingUserId);
        source.AdjustBalance(-amount);

        paymentLedgerAccount.MarkAsUsed();
    }

}
