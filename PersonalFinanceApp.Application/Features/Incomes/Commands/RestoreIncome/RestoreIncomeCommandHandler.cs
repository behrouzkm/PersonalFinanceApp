using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.Incomes.Commands.RestoreIncome;

public class RestoreIncomeCommandHandler : IRequestHandler<RestoreIncomeCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILedgerBalanceValidationService _ledgerBalanceValidation;

    public RestoreIncomeCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser,
                    ILedgerBalanceValidationService ledgerBalanceValidation)
    {
        _context = context;
        _currentUser = currentUser;
        _ledgerBalanceValidation = ledgerBalanceValidation;
    }

    public async Task Handle(RestoreIncomeCommand request, CancellationToken cancellationToken)
    {
        var document = await _context.AccountingDocuments
                .IgnoreQueryFilters()
                .Include(d => d.Entries)
                .FirstOrDefaultAsync(d => d.TenantId == _currentUser.TenantId &&
                        d.Id == request.AccountingDocumentId &&
                        d.DocumentType == DocumentType.Income, cancellationToken)
            ?? throw new NotFoundException(nameof(AccountingDocument), request.AccountingDocumentId);

        document.Restore(_currentUser.UserId);

        var debitEntries = document.Entries.Where(d => d.Debit > 0).ToList();
        var debitEntriesLedgerAccountIds = debitEntries.Select(e => e.LedgerAccountId).ToHashSet();

        var monetaryAccounts = await _context.MonetaryAccounts
                .Where(m => debitEntriesLedgerAccountIds.Contains(m.LedgerAccountId))
                .ToDictionaryAsync(m => m.LedgerAccountId, cancellationToken);

        foreach (var entry in debitEntries)
        {
            if (monetaryAccounts.TryGetValue(entry.LedgerAccountId, out var monetaryAccount))
            {
                // just for validation of documentDate and monetaryAccounts openingDate
                await _ledgerBalanceValidation.ValidateAsync(monetaryAccount, document.DocumentDate,
                    entry.Debit, 0, replacingEntryId: request.AccountingDocumentId, cancellationToken);

                monetaryAccount.AdjustBalance(entry.Debit);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
