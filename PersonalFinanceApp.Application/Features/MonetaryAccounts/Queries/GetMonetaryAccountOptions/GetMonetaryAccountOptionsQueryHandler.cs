using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Features.Common;
using Microsoft.EntityFrameworkCore;


namespace PersonalFinanceApp.Application.Features.MonetaryAccounts.Queries.GetMonetaryAccountOptions;

public class GetMonetaryAccountOptionsQueryHandler : IRequestHandler<GetMonetaryAccountOptionsQuery, List<MonetaryAccountOptionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMonetaryAccountOptionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MonetaryAccountOptionDto>> Handle(GetMonetaryAccountOptionsQuery request,
                        CancellationToken cancellationToken)
    {
        var options = await _context.MonetaryAccounts
            .OrderBy(o => o.DisplayOrder)
            .Select(r => new MonetaryAccountOptionDto
            {
                MonetaryAccountId = r.Id,
                DisplayName = r.DisplayName,
                CurrencyName = r.Currency.Name,
                CurrencySymbol = r.Currency.Symbol,
                CurrentBalance = r.CurrentBalance,
                CurrencyDecimalPlaces = r.Currency.DecimalPlaces
            })
            .ToListAsync(cancellationToken);

        return options;
    }
}

