using MediatR;
using PersonalFinanceApp.Application.Features.BankAccounts.Common;

namespace PersonalFinanceApp.Application.Features.BankAccounts.Queries.GetBankAccountsOptions;


public class GetBankAccountsOptionsQuery : IRequest<List<BankAccountOptionDto>>
{
}
