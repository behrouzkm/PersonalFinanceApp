using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Features.CurrencyExchanges.Common;

public class CurrencyExchangeListItemDto
{
    public Guid Id { get; init; }
    public string FromCurrencyCode { get; init; } = null!;
    public decimal FromAmount { get; init; }
    public string ToCurrencyCode { get; init; } = null!;
    public decimal ToAmount { get; init; }
    public decimal ExchangeRate { get; init; }
    public DateOnly ExchangeDate { get; init; }
    public int AttachmentCount { get; init; }
}
