using MediatR;
using PersonalFinanceApp.Application.Features.LedgerAccounts.Common;


namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Queries.GetLedgerAccountsOptions;

// One flexible query with optional filters, rather than a separate query per filter
// axis - covers listing, date-range reporting, and account/person-based views at once.
public class GetLedgerAccountsOptionsQuery : IRequest<List<LedgerAccountOptionDto>>
{
    public int? AccountTypeId { get; set; } = null;
    public Guid? ParentId { get; set; } = null;
}
