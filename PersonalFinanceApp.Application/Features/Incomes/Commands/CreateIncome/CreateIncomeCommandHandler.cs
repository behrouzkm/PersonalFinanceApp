using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.Incomes.Commands.CreateIncome;

public class CreateIncomeCommandHandler : IRequestHandler<CreateIncomeCommand, Guid>
{

    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAccountingLookupService _lookup;
    private readonly ILedgerBalanceValidationService _ledgerValidator;

    public CreateIncomeCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IAccountingLookupService lookup,
        ILedgerBalanceValidationService ledgerValidator)
    {
        _context = context;
        _currentUser = currentUserService;
        _lookup = lookup;
        _ledgerValidator = ledgerValidator;
    }


    public async Task<Guid> Handle(CreateIncomeCommand request, CancellationToken cancellationToken)
    {
        var incomeAccountIds = request.IncomeLedgerAccountLines
            .Select(l => l.LedgerAccountId)
            .Distinct()
            .ToList();

        var incomeLedgerAccounts = await _lookup.GetLedgerAccountsAsync(incomeAccountIds, cancellationToken);

        foreach (var id in incomeAccountIds)
        {
            if (!incomeLedgerAccounts.TryGetValue(id, out var account))
                throw new NotFoundException(nameof(LedgerAccount), id);

            if (!account.IsPostingAccount)
                throw new BusinessRuleException(ApplicationErrorCodes.Income.IncomeAccountNotPostable,
                    account.Id, account.Name);
        }

        var monetaryAccountIds = request.MonetaryAccountEntries
            .Select(p => p.MonetaryAccountId)
            .Distinct()
            .ToList();

        // Single lookup gives both dictionaries correctly - ById for resolving the DTO's
        // own MonetaryAccountId, ByLedgerAccountId for anything keyed off an entry's
        // LedgerAccountId later. This is what the old hand-rolled query got wrong.
        var monetaryAccountLookup = await _lookup.GetMonetaryAccountsAsync(
            monetaryAccountIds, Enumerable.Empty<Guid>(), cancellationToken);

        foreach (var deposit in request.MonetaryAccountEntries)
        {
            if (!monetaryAccountLookup.ById.ContainsKey(deposit.MonetaryAccountId))
                throw new NotFoundException(nameof(MonetaryAccount), deposit.MonetaryAccountId);
        }

        var income = new AccountingDocument(
            DocumentType.Income,
            request.DocumentDate,
            request.CurrencyId,
            _currentUser.TenantId,
            _currentUser.UserId,
            request.Description);

        foreach (var line in request.IncomeLedgerAccountLines)
        {
            income.AddEntry(line.LedgerAccountId, 0, line.Amount, line.Description ?? string.Empty, _currentUser.UserId);

            incomeLedgerAccounts[line.LedgerAccountId].MarkAsUsed();
        }

        foreach (var deposit in request.MonetaryAccountEntries)
        {
            var monetaryAccount = monetaryAccountLookup.ById[deposit.MonetaryAccountId];

            income.EnsureCurrencyMatches(monetaryAccount.CurrencyId);

            await _ledgerValidator.ValidateAsync(
                monetaryAccount, request.DocumentDate, deposit.Amount, 0, replacingEntryId: null, cancellationToken);

            income.AddEntry(monetaryAccount.LedgerAccountId, deposit.Amount, 0, deposit.Description, _currentUser.UserId);

            monetaryAccount.LedgerAccount.MarkAsUsed();
            monetaryAccount.AdjustBalance(deposit.Amount);
        }

        _context.AccountingDocuments.Add(income);
        await _context.SaveChangesAsync(cancellationToken);

        return income.Id;
    }


}
