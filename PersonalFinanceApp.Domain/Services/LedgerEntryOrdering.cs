using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Domain.Services;

// Centralizes the chronological ordering rule in one place, so it's never duplicated
// as a separate OrderBy chain inside a handler or a future reporting query.
//
// Debit is ordered before Credit on the same date because, per this system's actual
// entry semantics (confirmed against ApplyPayment/ApplyPaymentAsync), Debit increases
// a fund source's balance (money arriving) and Credit decreases it (money leaving) -
// arriving is processed before leaving on a day with both.
public static class LedgerEntryOrdering
{
    public static IOrderedEnumerable<T> OrderChronologically<T>(this IEnumerable<T> entries)
                    where T : IledgerEntryPoint
                => entries
                    .OrderBy(o => o.DocumentDate)
                    .ThenByDescending(o => o.Debit > 0)
                    .ThenBy(e => e.CreatedAt);
}
