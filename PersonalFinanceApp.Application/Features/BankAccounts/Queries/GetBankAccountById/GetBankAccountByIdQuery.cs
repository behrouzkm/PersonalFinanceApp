using MediatR;
using PersonalFinanceApp.Application.Features.BankAccounts.Common;


namespace PersonalFinanceApp.Application.Features.BankAccounts.Queries.GetBankAccountById;

public class GetBankAccountByIdQuery : IRequest<BankAccountDto>
{
    public Guid BankAccountId { get; set; }
}
