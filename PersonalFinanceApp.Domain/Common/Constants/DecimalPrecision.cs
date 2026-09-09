using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Domain.Common.Constants;

public static class DecimalPrecision
{
    public const int Precision = 18;
    public const int MonetaryAmountScale = 3;   // bounded by Currency.DecimalPlaces (max 3)
    public const int ExchangeRateScale = 6;      // unrelated to DecimalPlaces — a ratio, not an amount
}
