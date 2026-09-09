using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.BankAccounts.Commands.RestoreBankAccount;

public class RestoreBankAccountCommand : IRequest
{
    public Guid BankAccountId { get; set; }
}
