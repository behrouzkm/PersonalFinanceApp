using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Features.Common;

public class MonetaryAccountOptionDto
{
    public Guid MonetaryAccountId { get; set; }
    public string DisplayName { get; set; } = null!;
    public decimal CurrentBalance { get; set; }
    public string CurrencyName { get; set; } = null!;
    public string CurrencySymbol { get; set; } = null!;
    public int CurrencyDecimalPlaces { get; set; } = 2;
}
