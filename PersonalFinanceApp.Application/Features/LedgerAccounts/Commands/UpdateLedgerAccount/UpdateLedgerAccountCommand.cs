using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.UpdateLedgerAccount;

public class UpdateLedgerAccountCommand : IRequest
{
    public Guid LedgerAccountId { get; set; }
    public string Name { get; set; } = string.Empty!;

    public string? Description { get; set; }

    // The RowVersion the client last read.
    // Used to detect if someone else edited this ledger account in the meantime.
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
