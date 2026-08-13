using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.Incomes.Commands.DeleteIncome;

public class DeleteIncomeCommandHandler : IRequestHandler<DeleteIncomeCommand>
{
    public readonly IApplicationDbContext _context;
    public readonly ICurrentUserService _currentUser;

    public DeleteIncomeCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUser = currentUserService;
    }
    public async Task Handle(DeleteIncomeCommand request, CancellationToken cancellationToken)
    {
        var document = await _context.AccountingDocuments
            .Include(d => d.Entries)
            .FirstOrDefaultAsync(d => d.Id == request.AccountingDocumentId, cancellationToken)
            ?? throw new NotFoundException(nameof(AccountingDocument), request.AccountingDocumentId);

        // row version check for concurrency control
        _context.Entry(document).Property(d => d.RowVersion).OriginalValue = request.RowVersion;

        var debitEntries = document.Entries.Where(e => e.Debit > 0).ToList();
        var debitEntriesLedgerAccountIds = debitEntries.Select(e => e.LedgerAccountId).ToHashSet();

        var monetaryAccounts = await _context.MonetaryAccounts
            .Where(ma => debitEntriesLedgerAccountIds.Contains(ma.LedgerAccountId))
            .ToDictionaryAsync(ma => ma.LedgerAccountId, cancellationToken);

        // reverse the debit entries
        foreach (var entry in debitEntries)
        {
            if (monetaryAccounts.TryGetValue(entry.LedgerAccountId, out var monetaryAccount))
            {
                monetaryAccount.AdjustBalance(-entry.Debit);
            }
        }

        // soft delete the document and its entries
        document.SoftDelete(_currentUser.UserId);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
