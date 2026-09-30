using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.BankAccounts.Common;


namespace PersonalFinanceApp.Application.Features.BankAccounts.Queries.GetBankAccountsOptions;

public class GetBankAccountsOptionsQueryHandler : IRequestHandler<GetBankAccountsOptionsQuery, List<BankAccountOptionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBankAccountsOptionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BankAccountOptionDto>> Handle(GetBankAccountsOptionsQuery request,
                        CancellationToken cancellationToken)
    {
        var query = _context.BankAccounts
            .Include(b => b.Currency)
            .AsNoTracking()
            .AsQueryable();

        if (request.CurrencyId.HasValue)
        {
            query = query.Where(b => b.CurrencyId == request.CurrencyId.Value);
        }

        var projections = await query
            .OrderBy(o => o.DisplayOrder)
            .Select(r => new BankAccountOptionDto
            {
                Id = r.Id,
                LedgerAccountId = r.LedgerAccountId,
                BankAccountType = r.BankAccountType,
                DisplayName = r.DisplayName,
                CurrencyName = r.Currency.Name,
                CurrencySymbol = r.Currency.Symbol,
                CurrentBalance = r.CurrentBalance,
                CurrencyDecimalPlaces = r.Currency.DecimalPlaces
            })
            .ToListAsync(cancellationToken);

        return projections;
    }
}
