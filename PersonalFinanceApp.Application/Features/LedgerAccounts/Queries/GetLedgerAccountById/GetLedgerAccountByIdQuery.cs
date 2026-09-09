using MediatR;
using PersonalFinanceApp.Application.Features.LedgerAccounts.Common;


namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Queries.GetLedgerAccountById;

public class GetLedgerAccountByIdQuery : IRequest<LedgerAccountDto>
{
    public Guid LedgerAccountId { get; set; }
}
