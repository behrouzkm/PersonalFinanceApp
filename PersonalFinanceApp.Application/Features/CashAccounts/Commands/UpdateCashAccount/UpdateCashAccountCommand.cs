using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.CashAccounts.Commands.UpdateCashAccount;

public class UpdateCashAccountCommand : IRequest
{
    public Guid CashAccountId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string CashName { get; set; } = string.Empty;
    public string? BranchName { get; set; }
    public string CashAccountNumber { get; set; } = string.Empty;
    public string? IBAN { get; set; }
    public Guid ParentLedgerId { get; set; }
    public DateOnly OpeningDate { get; set; }
    public decimal InitialBalance { get; set; }
    public decimal CreditLimit { get; set; }
    public int CurrencyId { get; set; }
    public string? Description { get; set; }

    public string Location { get; set; } = string.Empty;
    public bool IsPhysical { get; set; }

    // The RowVersion the client last read.
    // Used to detect if someone else edited this document in the meantime.
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
