using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.CashAccounts.Commands.RestoreCashAccount;

public class RestoreCashAccountCommand : IRequest
{
    public Guid CashAccountId { get; set; }
}
