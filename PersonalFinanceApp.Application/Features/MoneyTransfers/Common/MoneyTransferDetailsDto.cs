using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Features.Attachments.Common;
using PersonalFinanceApp.Application.Features.Common;

namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Common;

public class MoneyTransferDetailsDto
{
    public Guid AccountingDocumentId { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateOnly TransferDate { get; set; }
    public Guid? FromMonetaryAccountId { get; set; }
    public Guid? ToMonetaryAccountId { get; set; }
    public Guid? FromPersonId { get; set; }
    public Guid? ToPersonId { get; set; }
    public int CurrencyId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }

    public IReadOnlyList<AttachmentDto> Attachments { get; set; } = Array.Empty<AttachmentDto>();
}
