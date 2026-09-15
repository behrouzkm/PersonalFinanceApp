namespace PersonalFinanceApp.Application.Features.CashAccounts.Common;

public class CashAccountListItemDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty!;
    public Guid LedgerAccountId { get; set; }
    public DateOnly OpeningDate { get; set; }
    public decimal InitialBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public Guid? OpeningAccountingDocumentId { get; set; }
    public int CurrencyId { get; set; }
    public string CurrencyName { get; set; } = null!;
    public string CurrencySymbol { get; set; } = null!;
    public int DisplayOrder { get; set; }
    public string Location { get; set; } = string.Empty;
    public bool IsPhysical { get; set; }
    public int AttachmentCount { get; set; }
}
