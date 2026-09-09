using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Features.Attachments.Common;

namespace PersonalFinanceApp.Application.Features.CurrencyExchanges.Common;

public class CurrencyExchangeDto
{
    public Guid Id { get; init; }
    public byte[] RowVersion { get; init; } = null!;

    public Guid FromLedgerAccountId { get; init; }
    public string FromFundSourceName { get; init; } = null!;
    public int FromCurrencyId { get; init; }
    public string FromCurrencyCode { get; init; } = null!;
    public decimal FromAmount { get; init; }

    public Guid ToLedgerAccountId { get; init; }
    public string ToFundSourceName { get; init; } = null!;
    public int ToCurrencyId { get; init; }
    public string ToCurrencyCode { get; init; } = null!;
    public decimal ToAmount { get; init; }

    public decimal ExchangeRate { get; init; }
    public DateOnly ExchangeDate { get; init; }
    public string? Description { get; init; }
    public IReadOnlyList<AttachmentDto> Attachments { get; set; } = Array.Empty<AttachmentDto>();

}
