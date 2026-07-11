using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.Expenditures.Commands.DeleteExpenditure;

public class DeleteExpenditureCommandHandler : IRequestHandler<DeleteExpenditureCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteExpenditureCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }
    public async Task Handle(DeleteExpenditureCommand request, CancellationToken cancellationToken)
    {
        var document = await _context.AccountingDocuments
            .Include(d => d.Entries)
            .FirstOrDefaultAsync(d => d.Id == request.AccountingDocumentId, cancellationToken)
            ?? throw new NotFoundException(nameof(AccountingDocument), request.AccountingDocumentId);

        // row version check for concurrency control
        _context.Entry(document).Property(d => d.RowVersion).OriginalValue = request.RowVersion;

        var creditEntries = document.Entries.Where(e => e.Credit > 0).ToList();
        var creditEntriesLedgerAccountIds = creditEntries.Select(e => e.LedgerAccountId).ToHashSet();

        var monetaryAccounts = await _context.MonetaryAccounts
            .Where(ma => creditEntriesLedgerAccountIds.Contains(ma.LedgerAccountId))
            .ToDictionaryAsync(ma => ma.LedgerAccountId, cancellationToken);

        var personAccounts = await _context.Persons
            .Where(pa => creditEntriesLedgerAccountIds.Contains(pa.LedgerAccountId))
            .ToDictionaryAsync(pa => pa.LedgerAccountId, cancellationToken);

        // reverse the credit entries
        foreach (var entry in creditEntries)
        {
            if (monetaryAccounts.TryGetValue(entry.LedgerAccountId, out var monetaryAccount))
            {
                monetaryAccount.AdjustBalance(entry.Credit);
            }
            else if (personAccounts.TryGetValue(entry.LedgerAccountId, out var personAccount))
            {
                personAccount.AdjustBalance(entry.Credit);
            }
        }

        // soft delete the document and its entries
        document.SoftDelete(_currentUser.UserId);

        await _context.SaveChangesAsync(cancellationToken);

    }
}
