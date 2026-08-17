using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Application.Common.Models;

public sealed class FundSourceLookup<TSource> where TSource : class, IFundSource
{
    public required Dictionary<Guid, TSource> ById { get; init; }
    public required Dictionary<Guid, TSource> ByLedgerAccountId { get; init; }
}
