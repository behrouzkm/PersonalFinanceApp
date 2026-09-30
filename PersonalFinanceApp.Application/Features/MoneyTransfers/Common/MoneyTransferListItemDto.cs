namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Common;

public sealed class MoneyTransferListItemDto
{
    public Guid MoneyTransferDocumentId { get; init; }

    public DateOnly TransferDate { get; init; }

    public Guid? FromLedgerAccountId { get; init; }
    public Guid? ToLedgerAccountId { get; init; }

    public string FromAccountName { get; init; } = string.Empty;
    public string ToAccountName { get; init; } = string.Empty;

    public int CurrencyId { get; init; }
    public string CurrencySymbol { get; init; } = string.Empty;
    public byte CurrencyDecimalPlaces { get; init; } = 2;

    public decimal Amount { get; init; }

    public string? Description { get; init; }

    public int AttachmentCount { get; init; }

    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public string? CreatedByUserName { get; set; }
}
