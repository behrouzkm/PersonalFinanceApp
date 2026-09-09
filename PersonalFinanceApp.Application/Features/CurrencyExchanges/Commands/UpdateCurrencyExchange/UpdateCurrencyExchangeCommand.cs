using MediatR;

namespace PersonalFinanceApp.Application.Features.CurrencyExchanges.Commands.UpdateCurrencyExchange;

public class UpdateCurrencyExchangeCommand : IRequest
{
    public Guid CurrencyExchangeId { get; init; }
    public byte[] RowVersion { get; init; } = null!;
    public Guid FromLedgerAccountId { get; init; }
    public Guid ToLedgerAccountId { get; init; }
    public decimal FromAmount { get; init; }
    public decimal ToAmount { get; init; }
    public decimal ExchangeRate { get; init; }
    public DateOnly ExchangeDate { get; init; }
    public string? Description { get; init; }

}
