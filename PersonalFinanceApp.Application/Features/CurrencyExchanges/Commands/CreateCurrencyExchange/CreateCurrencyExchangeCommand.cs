using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Features.Common;

namespace PersonalFinanceApp.Application.Features.CurrencyExchanges.Commands.CreateCurrencyExchange;

public class CreateCurrencyExchangeCommand : IRequest<Guid>
{
    public Guid FromLedgerAccountId { get; init; }
    public Guid ToLedgerAccountId { get; init; }
    public decimal FromAmount { get; init; }
    public decimal ToAmount { get; init; }
    public decimal ExchangeRate { get; init; }
    public DateOnly ExchangeDate { get; init; }
    public string? Description { get; init; }
}
