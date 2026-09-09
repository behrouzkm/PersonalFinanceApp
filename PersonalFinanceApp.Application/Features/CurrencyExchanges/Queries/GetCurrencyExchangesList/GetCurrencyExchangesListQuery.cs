using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.CurrencyExchanges.Common;

namespace PersonalFinanceApp.Application.Features.CurrencyExchanges.Queries.GetCurrencyExchangesList;

public class GetCurrencyExchangesListQuery  : IRequest<PaginatedList<CurrencyExchangeListItemDto>>
{
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
    public Guid? LedgerAccountId { get; set; }
    public int? FromCurrencyId { get; set; }
    public int? ToCurrencyId { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

