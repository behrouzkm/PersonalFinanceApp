using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.Common;
using PersonalFinanceApp.Domain.Entities;


namespace PersonalFinanceApp.Application.Features.Currencies.Queries.GetCurrenciesList;

public class GetCurrencyListQueryHandler : IRequestHandler<GetCurrenciesListQuery, PaginatedList<CurrencyDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCurrencyListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<CurrencyDto>> Handle(GetCurrenciesListQuery request,
                        CancellationToken cancellationToken)
    {
        var query = _context.Currencies;

        var projection = query
            .OrderBy(o => o.Code)
            .Select(r => new CurrencyDto
            {
                Id = r.Id,
                Name = r.Name,
                Code = r.Code,
                DecimalPlaces = r.DecimalPlaces,
                Symbol = r.Symbol
            });

        return await PaginatedList<CurrencyDto>.CreateAsync(projection, request.PageNumber,
                       request.PageSize, cancellationToken);
    }
}
