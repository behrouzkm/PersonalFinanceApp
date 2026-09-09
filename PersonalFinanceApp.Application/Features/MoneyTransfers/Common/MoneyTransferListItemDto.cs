namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Common;

public class MoneyTransferListItemDto
{
    public Guid AccountingDocumentId { get; set; }
    public DateOnly TransferDate { get; set; }
    public Guid? FromLedgerAccountId { get; set; }
    public Guid? ToLedgerAccountId { get; set; }
    public int CurrencyId { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public int AttachmentCount { get; set; }
}
