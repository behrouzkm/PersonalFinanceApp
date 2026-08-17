using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Features.Common;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.Incomes.Queries.GetIncomeById;

public class GetIncomeByIdQueryHandler : IRequestHandler<GetIncomeByIdQuery, IncomeDetailsDto>
{
    private readonly IApplicationDbContext _context;

    public GetIncomeByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IncomeDetailsDto> Handle(GetIncomeByIdQuery request, CancellationToken cancellationToken)
    {
        var document = await _context.AccountingDocuments
            .Include(d => d.Entries)
            .FirstOrDefaultAsync(d => d.Id == request.AccountingDocumentId
                && d.DocumentType == DocumentType.Income, cancellationToken)
            ?? throw new NotFoundException(nameof(AccountingDocument), request.AccountingDocumentId);

        var activeEntries = document.Entries.Where(e => !e.IsDeleted).ToList();
        var ledgerAccountIds = activeEntries.Select(e => e.LedgerAccountId).Distinct().ToList();

        var monetaryAccountsByLedgerId = await _context.MonetaryAccounts
            .Where(m => ledgerAccountIds.Contains(m.LedgerAccountId))
            .ToDictionaryAsync(m => m.LedgerAccountId, cancellationToken);

        var dto = new IncomeDetailsDto
        {
            AccountingDocumentId = document.Id,
            RowVersion = document.RowVersion,
            DocumentDate = document.DocumentDate,
            CurrencyId = document.CurrencyId,
            Description = document.Description
        };

        foreach (var entry in activeEntries)
        {
            if (entry.Credit > 0)
            {
                // the income category (credit) side
                dto.IncomeLedgerAccountLines.Add(new AccountingEntryDto
                {
                    AccountingEntryId = entry.Id,
                    LedgerAccountId = entry.LedgerAccountId,
                    Amount = entry.Credit,
                    Description = entry.Description
                });
            }
            else if (monetaryAccountsByLedgerId.TryGetValue(entry.LedgerAccountId, out var monetaryAccount))
            {
                // the deposit (debit) side
                dto.MonetaryAccountEntries.Add(new MonetaryAccountEntryDto
                {
                    AccountingEntryId = entry.Id,
                    MonetaryAccountId = monetaryAccount.Id,
                    Amount = entry.Debit,
                    Description = entry.Description
                });
            }
        }

        return dto;
    }
}
