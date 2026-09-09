using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.CashAccounts.Common;


namespace PersonalFinanceApp.Application.Features.CashAccounts.Queries.GetCashAccountsOptions;

public class GetCashAccountsOptionsQueryHandler : IRequestHandler<GetCashAccountsOptionsQuery, List<CashAccountOptionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCashAccountsOptionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CashAccountOptionDto>> Handle(GetCashAccountsOptionsQuery request,
                        CancellationToken cancellationToken)
    {
        var options = await _context.CashAccounts
            .Include(b => b.Currency)
            .AsNoTracking()
            .OrderBy(o => o.DisplayOrder)
            .Select(r => new CashAccountOptionDto
            {
                Id = r.Id,
                DisplayName = r.DisplayName,
                CurrencyName = r.Currency.Name,
                CurrencySymbol = r.Currency.Symbol,
                CurrentBalance = r.CurrentBalance
            })
            .ToListAsync(cancellationToken);

        return options;
    }
}
