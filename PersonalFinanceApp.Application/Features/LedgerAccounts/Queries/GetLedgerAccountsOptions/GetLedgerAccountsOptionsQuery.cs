using MediatR;
using PersonalFinanceApp.Application.Features.LedgerAccounts.Common;
using PersonalFinanceApp.Domain.Enums;


namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Queries.GetLedgerAccountsOptions;

// One flexible query with optional filters, rather than a separate query per filter
// axis - covers listing, date-range reporting, and account/person-based views at once.
public class GetLedgerAccountsOptionsQuery : IRequest<List<LedgerAccountOptionDto>>
{
    public bool GetJustPostingAccount { get; set; } = true;
     public IEnumerable<AccountCategory>? AccountCategories { get; set; } = null;
    public Guid? ParentId { get; set; } = null;

}
