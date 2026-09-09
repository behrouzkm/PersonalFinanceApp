using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.CreateLedgerAccount;

public class CreateLedgerAccountCommand : IRequest<Guid>
{
    public int AccountTypeId { get; set; }
    public string Name { get; set; } = string.Empty!;

    // Link to the parent
    public Guid ParentId { get; set; }
    public string? Description { get; set; }
}
