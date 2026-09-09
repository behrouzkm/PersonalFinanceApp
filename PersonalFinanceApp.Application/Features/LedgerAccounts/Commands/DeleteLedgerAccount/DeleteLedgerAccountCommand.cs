using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.DeleteLedgerAccount;

public class DeleteLedgerAccountCommand : IRequest
{
    public Guid LedgerAccountId { get; set; }

}
