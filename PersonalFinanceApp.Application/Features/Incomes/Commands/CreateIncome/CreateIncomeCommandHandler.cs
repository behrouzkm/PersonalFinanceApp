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

    public CreateIncomeCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUser = currentUserService;
    }
    public async Task<Guid> Handle(CreateIncomeCommand request, CancellationToken cancellationToken)
    {
        var incomeAccountIds = request.IncomeLedgerAccountLines
            .Select(l => l.LedgerAccountId)
            .Distinct()
            .ToList();

        var incomeLedgerAccounts = await _context.LedgerAccounts
            .Where(a => incomeAccountIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, cancellationToken);

        foreach (var id in incomeAccountIds)
        {
            if (!incomeLedgerAccounts.TryGetValue(id, out var account))
                throw new NotFoundException(nameof(LedgerAccount), id);

            if (!account.IsPostingAccount)
                throw new BusinessRuleException(ApplicationErrorCodes.Income.IncomeAccountNotPostable,
                    account.Id, account.Name);
        }

        var monetaryAccountIds = request.MonetaryAccountEntries
            .Select(p => p.MonetaryLedgerAccountId)
            .Distinct()
            .ToList();

        var monetaryAccounts = await _context.MonetaryAccounts
             .Where(a => monetaryAccountIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, cancellationToken);

        var monetaryLedgerAccounts = await _context.LedgerAccounts
            .Where(a => monetaryAccountIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, a => a, cancellationToken);

        foreach (var id in request.MonetaryAccountEntries)
        {
            if (!monetaryAccounts.TryGetValue(id.MonetaryLedgerAccountId, out var account))
                throw new NotFoundException(nameof(MonetaryAccount), id.MonetaryLedgerAccountId);
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
            var monetaryAccount = monetaryAccounts[deposit.MonetaryLedgerAccountId];
            var monetaryLedgerAccount = monetaryLedgerAccounts[deposit.MonetaryLedgerAccountId];

            income.EnsureCurrencyMatches(monetaryAccount.CurrencyId);
            income.AddEntry(deposit.MonetaryLedgerAccountId, 0, deposit.Amount, deposit.Description, _currentUser.UserId);

            monetaryLedgerAccount.MarkAsUsed();

            monetaryAccount.AdjustBalance(deposit.Amount);
        }

        _context.AccountingDocuments.Add(income);
        await _context.SaveChangesAsync(cancellationToken);

        return income.Id;
    }


}
