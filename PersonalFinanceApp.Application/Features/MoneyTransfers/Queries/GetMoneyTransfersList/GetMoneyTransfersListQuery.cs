using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.MoneyTransfers.Common;

namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Queries.GetMoneyTransfersList;

// One flexible query with optional filters, rather than a separate query per filter
// axis - covers listing, date-range reporting, and account/person-based views at once.
public class GetMoneyTransfersListQuery : IRequest<PaginatedList<MoneyTransferListItemDto>>
{
    public string? SearchText { get; set; }

    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }

    public Guid? MonetaryAccountId { get; set; }
    public Guid? PersonId { get; set; }
    public Guid? LedgerAccountId { get; set; }

    public int? CurrencyId { get; set; }

    public decimal? FromAmount { get; set; }
    public decimal? ToAmount { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

}
