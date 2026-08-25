using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Features.Common;

public class CurrencyDto
{
    public byte Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public byte DecimalPlaces { get; set; }
    public string Symbol { get; set; }
}
