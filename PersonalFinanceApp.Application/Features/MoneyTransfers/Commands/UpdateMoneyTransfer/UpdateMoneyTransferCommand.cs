using MediatR;

namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Commands.UpdateMoneyTransfer;

public class UpdateMoneyTransferCommand : IRequest
{
    public Guid MoneyTransferDocumentId { get; set; }

    // The RowVersion the client last read (e.g. from a GetMoneyTransferById query).
    // Used to detect if someone else edited this document in the meantime.
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateOnly TransferDate { get; set; }
    public Guid FromLedgerAccountId { get; init; }
    public Guid ToLedgerAccountId { get; init; }
    // public Guid FromMonetaryAccountId { get; set; }
    // public Guid ToMonetaryAccountId { get; set; }
    public int CurrencyId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }

}
