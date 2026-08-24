using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.Expenditures.Queries.GetExpendituresList;

public class GetExpenditureListQueryHandler : IRequestHandler<GetExpendituresListQuery, PaginatedList<ExpenditureListItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetExpenditureListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<ExpenditureListItemDto>> Handle(GetExpendituresListQuery request,
                        CancellationToken cancellationToken)
    {
        var query = _context.AccountingDocuments
            .Where(d => d.DocumentType == DocumentType.Expenditure);

        if (request.FromDate.HasValue)
            query = query.Where(d => d.DocumentDate >= request.FromDate);

        if (request.ToDate.HasValue)
            query = query.Where(d => d.DocumentDate <= request.ToDate);

        if (request.LedgerAccountId.HasValue)
            query = query.Where(d => d.Entries.Any(e => e.LedgerAccountId == request.LedgerAccountId.Value));

        if (request.MonetaryAccountId.HasValue)
        {
            var ledgerAccountId = await _context.MonetaryAccounts
                .Where(m => m.Id == request.MonetaryAccountId.Value)
                .Select(m => (Guid?)m.LedgerAccountId)
                .FirstOrDefaultAsync(cancellationToken);

            query = query.Where(d => d.Entries.Any(e => e.LedgerAccountId == ledgerAccountId));
        }

        if (request.PersonId.HasValue)
        {
            var ledgerAccountId = await _context.Persons
                .Where(p => p.Id == request.PersonId.Value)
                .Select(p => (Guid?)p.LedgerAccountId)
                .FirstOrDefaultAsync(cancellationToken);

            query = query.Where((d => d.Entries.Any(e => e.LedgerAccountId == ledgerAccountId)));
        }

        var projected = query
            .OrderByDescending(d => d.DocumentDate)
            .ThenByDescending(d => d.CreatedAt)
            .Select(d => new ExpenditureListItemDto
            {
                AccountingDocumentId = d.Id,
                DocumentDate = d.DocumentDate,
                CurrencyId = d.CurrencyId,
                Description = d.Description,
                TotalAmount = d.Entries.Where(d => d.Debit > 0).Sum(d => d.Debit)
            });

        return await PaginatedList<ExpenditureListItemDto>.CreateAsync(projected, request.PageNumber,
                        request.PageSize, cancellationToken);
    }
}
