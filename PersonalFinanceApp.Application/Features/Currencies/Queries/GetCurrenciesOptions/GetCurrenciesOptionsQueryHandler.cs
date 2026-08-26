using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.Currencies.Common;


namespace PersonalFinanceApp.Application.Features.Currencies.Queries.GetCurrenciesOptions;

public class GetCurrenciesOptionsQueryHandler : IRequestHandler<GetCurrenciesOptionsQuery, List<CurrencyOptionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCurrenciesOptionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CurrencyOptionDto>> Handle(GetCurrenciesOptionsQuery request,
                        CancellationToken cancellationToken)
    {
        var options = await _context.Currencies
            .AsNoTracking()
            .Where(r => r.IsActive)
            .OrderBy(o => o.DisplayOrder)
            .Select(r => new CurrencyOptionDto
            {
                Id = r.Id,
                Name = r.Name,
                Code = r.Code,
                DecimalPlaces = r.DecimalPlaces,
                Symbol = r.Symbol
            })
            .ToListAsync(cancellationToken);

        return options;
    }
}
