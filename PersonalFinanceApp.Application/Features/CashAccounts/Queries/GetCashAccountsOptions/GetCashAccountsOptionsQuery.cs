using MediatR;
using PersonalFinanceApp.Application.Features.CashAccounts.Common;

namespace PersonalFinanceApp.Application.Features.CashAccounts.Queries.GetCashAccountsOptions;


public class GetCashAccountsOptionsQuery : IRequest<List<CashAccountOptionDto>>
{
}
