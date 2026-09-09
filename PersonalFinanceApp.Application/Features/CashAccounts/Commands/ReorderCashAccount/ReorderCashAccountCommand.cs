using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.CashAccounts.Commands.ReorderCashAccount;

public class ReorderCashAccountCommand : IRequest
{
    public Guid CashAccountId { get; set; }
    public int NewDisplayOrder { get; set; }
}
