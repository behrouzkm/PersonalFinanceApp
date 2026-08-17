using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.Expenditures.Commands.RestoreExpenditure;

public class RestoreExpenditureCommandHandler : IRequestHandler<RestoreExpenditureCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILedgerBalanceValidationService _ledgerValidator;

    public RestoreExpenditureCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser,
                ILedgerBalanceValidationService ledgerValidator)
    {
        _context = context;
        _currentUser = currentUser;
        _ledgerValidator = ledgerValidator;
    }
    public async Task Handle(RestoreExpenditureCommand request, CancellationToken cancellationToken)
    {
        var document = await _context.AccountingDocuments
            .IgnoreQueryFilters()
            .Include(d => d.Entries)
            .FirstOrDefaultAsync(d => d.Id == request.AccountingDocumentId
                && d.DocumentType == Domain.Enums.DocumentType.Expenditure
                && d.TenantId == _currentUser.TenantId, cancellationToken)
            ?? throw new NotFoundException(nameof(AccountingDocument), request.AccountingDocumentId);

        document.Restore(_currentUser.UserId);

        var creditEntries = document.Entries.Where(e => e.Credit > 0).ToList();
        var ledgerAccountIds = creditEntries.Select(s => s.LedgerAccountId).Distinct().ToList();

        var monetaryAccounts = await _context.MonetaryAccounts
            .Where(m => ledgerAccountIds.Contains(m.LedgerAccountId))
            .ToDictionaryAsync(m => m.LedgerAccountId, cancellationToken);

        var persons = await _context.Persons
            .Where(p => ledgerAccountIds.Contains(p.LedgerAccountId))
            .ToDictionaryAsync(p => p.LedgerAccountId, cancellationToken);

        foreach (var entry in creditEntries)
        {
            if (monetaryAccounts.TryGetValue(entry.LedgerAccountId, out var account))
            {
                await _ledgerValidator.ValidateAsync(account, document.DocumentDate, 0, entry.Credit,
                    replacingEntryId: document.Id, cancellationToken);

                account.AdjustBalance(-entry.Credit);
            }
            else if (persons.TryGetValue(entry.LedgerAccountId, out var person))
            {
                await _ledgerValidator.ValidateAsync(person, document.DocumentDate, 0, entry.Credit,
                    replacingEntryId: document.Id, cancellationToken);

                person.AdjustBalance(-entry.Credit);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
