using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.Incomes.Queries.GetIncomesList;

public class GetIncomesListQueryHandler : IRequestHandler<GetIncomesListQuery, PaginatedList<IncomeListItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetIncomesListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<IncomeListItemDto>> Handle(GetIncomesListQuery request, CancellationToken cancellationToken)
    {
        var query = _context.AccountingDocuments
            .Where(d => d.DocumentType == DocumentType.Income && !d.IsDeleted);

        if (request.FromDate.HasValue)
            query = query.Where(d => d.DocumentDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(d => d.DocumentDate <= request.ToDate.Value);

        if (request.LedgerAccountId.HasValue)
            query = query.Where(d => d.Entries.Any(e => !e.IsDeleted && e.LedgerAccountId == request.LedgerAccountId.Value));

        if (request.MonetaryAccountId.HasValue)
        {
            var ledgerAccountId = await _context.MonetaryAccounts
                .Where(m => m.Id == request.MonetaryAccountId.Value)
                .Select(m => (Guid?)m.LedgerAccountId)
                .FirstOrDefaultAsync(cancellationToken);

            query = query.Where(d => d.Entries.Any(e => !e.IsDeleted && e.LedgerAccountId == ledgerAccountId));
        }

        var projected = query
            .OrderByDescending(d => d.DocumentDate)
            .ThenByDescending(d => d.CreatedAt)
            .Select(d => new IncomeListItemDto
            {
                AccountingDocumentId = d.Id,
                DocumentDate = d.DocumentDate,
                CurrencyId = d.CurrencyId,
                Description = d.Description,
                TotalAmount = d.Entries.Where(e => !e.IsDeleted && e.Credit > 0).Sum(e => e.Credit)
            });

        return await PaginatedList<IncomeListItemDto>.CreateAsync(
            projected, request.PageNumber, request.PageSize, cancellationToken);
    }
}
