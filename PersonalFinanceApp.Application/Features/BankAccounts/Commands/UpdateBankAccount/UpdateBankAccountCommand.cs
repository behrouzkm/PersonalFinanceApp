using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.BankAccounts.Commands.UpdateBankAccount;

public class UpdateBankAccountCommand : IRequest
{
    public Guid BankAccountId { get; set; }
    public BankAccountType BankAccountType { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string? BranchName { get; set; }
    public string BankAccountNumber { get; set; } = string.Empty;
    public string? IBAN { get; set; }
    public Guid ParentLedgerId { get; set; }
    public DateOnly OpeningDate { get; set; }
    public decimal InitialBalance { get; set; }
    public decimal CreditLimit { get; set; }
    public int CurrencyId { get; set; }
    public string? Description { get; set; }

    // The RowVersion the client last read.
    // Used to detect if someone else edited this document in the meantime.
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
