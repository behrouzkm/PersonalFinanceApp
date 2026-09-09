using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.LedgerAccounts.Common;


namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Queries.GetLedgerAccountsOptions;

public class GetLedgerAccountsOptionsQueryHandler : IRequestHandler<GetLedgerAccountsOptionsQuery, List<LedgerAccountOptionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLedgerAccountsOptionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LedgerAccountOptionDto>> Handle(GetLedgerAccountsOptionsQuery request,
                        CancellationToken cancellationToken)
    {

        var query = _context.LedgerAccounts.AsNoTracking();

        if (request.AccountTypeId.HasValue)
            query = query.Where(r => r.AccountTypeId == request.AccountTypeId.Value);

        if (request.ParentId.HasValue)
            query = query.Where(r => r.ParentId == request.ParentId.Value);

        var results = query
            .OrderBy(o => o.DisplayOrder)
            .Select(r => new LedgerAccountOptionDto
            {
                Id = r.Id,
                Name = r.Name,
                ParentId = r.ParentId!.Value
            })
            .ToListAsync(cancellationToken);

        return await results;
    }
}
