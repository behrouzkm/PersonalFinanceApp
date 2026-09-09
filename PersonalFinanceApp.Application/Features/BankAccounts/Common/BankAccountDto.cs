using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Features.Attachments.Common;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.BankAccounts.Common;

public class BankAccountDto
{
    public Guid BankId { get; set; }
    public BankAccountType BankAccountType { get; set; }
    public string DisplayName { get; set; } = string.Empty!;
    public Guid LedgerAccountId { get; set; }
    public DateOnly OpeningDate { get; set; }
    public decimal InitialBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal? CreditLimit { get; set; }
    public Guid? OpeningAccountingDocumentId { get; set; }
    public int CurrencyId { get; set; }
    public string CurrencyName { get; set; } = null!;
    public string CurrencySymbol { get; set; } = null!;
    public int DisplayOrder { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string? BranchName { get; set; }
    public string BankAccountNumber { get; set; } = string.Empty;
    public string? IBAN { get; set; }
    public IReadOnlyList<AttachmentDto> Attachments { get; set; } = Array.Empty<AttachmentDto>();
}
