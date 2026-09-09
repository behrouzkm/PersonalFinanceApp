using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.ReorderLedgerAccount;

public class ReorderLedgerAccountCommand : IRequest
{
    public Guid LedgerAccountId { get; set; }
    public Guid ParentId { get; set; }
    public int NewDisplayOrder { get; set; }
}
