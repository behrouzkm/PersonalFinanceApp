using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Features.Attachments.Common;
using PersonalFinanceApp.Application.Features.Common;

namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Common;

public class MoneyTransferDetailsDto
{
    public Guid MoneyTransferDocumentId { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateOnly TransferDate { get; set; }

    public Guid FromLedgerAccountId { get; set; }
    public Guid ToLedgerAccountId { get; set; }

    public Guid? FromPersonId { get; set; }
    public Guid? FromBankAccountId { get; set; }
    public Guid? FromCashAccountId { get; set; }

    public Guid? ToPersonId { get; set; }
    public Guid? ToBankAccountId { get; set; }
    public Guid? ToCashAccountId { get; set; }

    public int CurrencyId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;
    public string CurrencySymbol { get; set; } = string.Empty;
    public int CurrencyDecimalPlaces { get; set; }

    public string FromDisplayName { get; set; } = string.Empty;
    public string ToDisplayName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public string? CreatedByUserName { get; set; }

    public DateTime? LastModifiedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public string? LastModifiedByUserName { get; set; }

    public IReadOnlyList<AttachmentDto> Attachments { get; set; }
        = Array.Empty<AttachmentDto>();

}
