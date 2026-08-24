using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.Currencies.Commands.CreateCurrency;

public class CreateCurrencyCommand : IRequest<byte>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DecimalPlaces { get; set; }
    public string Symbol { get; set; } = string.Empty;
}
