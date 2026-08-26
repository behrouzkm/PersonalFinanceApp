using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Features.Currencies.Common;

public class CurrencyOptionDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public byte DecimalPlaces { get; set; }
    public string Symbol { get; set; } = string.Empty;
}
