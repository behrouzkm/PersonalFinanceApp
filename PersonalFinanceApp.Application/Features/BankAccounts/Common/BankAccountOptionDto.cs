using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.BankAccounts.Common;

public class BankAccountOptionDto
{
    public Guid Id { get; set; }
    public BankAccountType BankAccountType { get; set; }
    public string DisplayName { get; set; } = null!;
    public decimal CurrentBalance { get; set; }
    public string CurrencyName { get; set; } = null!;
    public string CurrencySymbol { get; set; } = null!;
}
