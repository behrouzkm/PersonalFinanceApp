using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.BankAccounts.Commands.CreateBankAccount;

public class CreateBankAccountCommand : IRequest<Guid>
{
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
}
