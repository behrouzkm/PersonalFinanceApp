using MediatR;
using PersonalFinanceApp.Application.Common.Models;

namespace PersonalFinanceApp.Application.Features.Incomes.Queries.GetIncomesList;

// No PersonId filter - Income has no person side in the current domain model.
public class GetIncomesListQuery : IRequest<PaginatedList<IncomeListItemDto>>
{
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
    public Guid? MonetaryAccountId { get; set; }
    public Guid? LedgerAccountId { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
