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
        var options = await _context.BankAccounts
            .Include(b => b.Currency)
            .AsNoTracking()
            .OrderBy(o => o.DisplayOrder)
            .Select(r => new BankAccountOptionDto
            {
                Id = r.Id,
                BankAccountType = r.BankAccountType,
                DisplayName = r.DisplayName,
                CurrencyName = r.Currency.Name,
                CurrencySymbol = r.Currency.Symbol,
                CurrentBalance = r.CurrentBalance
            })
            .ToListAsync(cancellationToken);

        return options;
    }
}
