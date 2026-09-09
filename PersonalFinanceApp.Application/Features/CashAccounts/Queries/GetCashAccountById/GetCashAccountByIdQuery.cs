using MediatR;
using PersonalFinanceApp.Application.Features.CashAccounts.Common;


namespace PersonalFinanceApp.Application.Features.CashAccounts.Queries.GetCashAccountById;

public class GetCashAccountByIdQuery : IRequest<CashAccountDto>
{
    public Guid CashAccountId { get; set; }
}
