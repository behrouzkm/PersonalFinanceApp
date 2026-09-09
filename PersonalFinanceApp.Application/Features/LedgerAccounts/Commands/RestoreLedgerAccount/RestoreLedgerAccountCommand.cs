using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.RestoreLedgerAccount;

public class RestoreLedgerAccountCommand : IRequest
{
    public Guid LedgerAccountId { get; set; }
}
