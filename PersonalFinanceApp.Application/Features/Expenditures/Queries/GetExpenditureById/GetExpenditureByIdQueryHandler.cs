using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Features.Common;
using PersonalFinanceApp.Application.Features.Expenditures.Common;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.Expenditures.Queries.GetExpenditureById;

public class GetExpenditureByIdQueryHandler : IRequestHandler<GetExpenditureByIdQuery, ExpenditureDetailsDto>
{
    private readonly IApplicationDbContext _context;

    public GetExpenditureByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ExpenditureDetailsDto> Handle(GetExpenditureByIdQuery request, CancellationToken cancellationToken)
    {
        var document = await _context.AccountingDocuments
                .Include(i => i.Entries)
                .FirstOrDefaultAsync(d => d.Id == request.AccountingDocumentId
                    && d.DocumentType == Domain.Enums.DocumentType.Expenditure, cancellationToken)
            ?? throw new NotFoundException(nameof(AccountingDocument), request.AccountingDocumentId);


        var activeEntries = document.Entries.ToList();
        var ledgerAccountIds = activeEntries.Select(s => s.LedgerAccountId).Distinct().ToList();

        // Resolve which ledger accounts belong to a MonetaryAccount vs a Person vs a
        // plain expense category, so each entry can be routed into the right list.
        var monetaryAccountByLedgerId = await _context.MonetaryAccounts
                .Where(m => ledgerAccountIds.Contains(m.LedgerAccountId))
                .ToDictionaryAsync(m => m.LedgerAccountId, cancellationToken);

        var personsByLedgerId = await _context.Persons
                 .Where(p => ledgerAccountIds.Contains(p.LedgerAccountId))
                 .ToDictionaryAsync(p => p.LedgerAccountId, cancellationToken);

        var dto = new ExpenditureDetailsDto
        {
            AccountingDocumentId = document.Id,
            RowVersion = document.RowVersion,
            DocumentDate = document.DocumentDate,
            CurrencyId = document.CurrencyId,
            Description = document.Description
        };

        foreach (var entry in activeEntries)
        {
            if (entry.Debit > 0)
            {
                dto.ExpenditureLedgerAccountLines.Add(new AccountingEntryDto
                {
                    AccountingEntryId = entry.Id,
                    LedgerAccountId = entry.LedgerAccountId,
                    Amount = entry.Debit,
                    Description = entry.Description
                });
            }
            else if (monetaryAccountByLedgerId.TryGetValue(entry.LedgerAccountId, out var monetaryAccount))
            {
                dto.MonetaryAccountEntries.Add(new MonetaryAccountEntryDto
                {
                    AccountingEntryId = entry.Id,
                    MonetaryAccountId = monetaryAccount.Id,
                    Amount = entry.Credit,
                    Description = entry.Description
                });
            }
            else if (personsByLedgerId.TryGetValue(entry.LedgerAccountId, out var person))
            {
                dto.PersonPaymentEntries.Add(new PersonPaymentDto
                {
                    AccountingEntryId = entry.Id,
                    PersonId = person.Id,
                    Amount = entry.Credit,
                    Description = entry.Description
                });
            }

        }

        return dto;


    }
}
