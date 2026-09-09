using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.CurrencyExchanges.Common;

namespace PersonalFinanceApp.Application.Features.CurrencyExchanges.Queries.GetCurrencyExchangesList;

public class GetCurrencyExchangesListQueryHandler : IRequestHandler<GetCurrencyExchangesListQuery, PaginatedList<CurrencyExchangeListItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCurrencyExchangesListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<CurrencyExchangeListItemDto>> Handle(
        GetCurrencyExchangesListQuery request, CancellationToken cancellationToken)
    {

        var query = _context.CurrencyExchanges
                .Include(r => r.FromDocument).ThenInclude(d => d.Currency)
                .Include(r => r.ToDocument).ThenInclude(d => d.Currency)
                .Include(r => r.FromDocument).ThenInclude(d => d.Entries)
                .Include(r => r.ToDocument).ThenInclude(d => d.Entries)
                .AsQueryable();

        if (request.FromDate.HasValue)
            query = query.Where(d => d.FromDocument.DocumentDate >= request.FromDate);

        if (request.ToDate.HasValue)
            query = query.Where(d => d.FromDocument.DocumentDate <= request.ToDate);

        if (request.LedgerAccountId.HasValue)
            query = query.Where(d => d.FromDocument.Entries.Any(e => e.LedgerAccountId == request.LedgerAccountId.Value)
                        || d.ToDocument.Entries.Any(e => e.LedgerAccountId == request.LedgerAccountId));

        if (request.FromCurrencyId.HasValue)
            query = query.Where(d => d.FromDocument.CurrencyId == request.FromCurrencyId);

        if (request.ToCurrencyId.HasValue)
            query = query.Where(d => d.ToDocument.CurrencyId == request.ToCurrencyId);

        var projected = query
            .OrderByDescending(o => o.FromDocument.DocumentDate)
            .Select(cx => new CurrencyExchangeListItemDto
            {
                Id = cx.Id,
                FromCurrencyCode = cx.FromDocument.Currency.Code,
                FromAmount = cx.FromDocument.Entries.First(d => d.Debit > 0).Debit,
                ToCurrencyCode = cx.ToDocument.Currency.Code,
                ToAmount = cx.ToDocument.Entries.First(d => d.Debit > 0).Debit,
                ExchangeRate = cx.ExchangeRate,
                ExchangeDate = cx.FromDocument.DocumentDate,
                AttachmentCount = _context.Attachments.Count(a => a.CurrencyExchangeId == cx.Id)
            });

        return await PaginatedList<CurrencyExchangeListItemDto>.CreateAsync(projected, request.PageNumber,
                        request.PageSize, cancellationToken);


    }
}
