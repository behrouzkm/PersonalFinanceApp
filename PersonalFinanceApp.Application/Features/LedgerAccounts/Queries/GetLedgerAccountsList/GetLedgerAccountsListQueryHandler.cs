using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.LedgerAccounts.Common;


namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Queries.GetLedgerAccountsList;

public class GetLedgerAccountsListQueryHandler : IRequestHandler<GetLedgerAccountsListQuery, PaginatedList<LedgerAccountDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLedgerAccountsListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<LedgerAccountDto>> Handle(GetLedgerAccountsListQuery request,
                        CancellationToken cancellationToken)
    {
        var query = _context.LedgerAccounts.AsQueryable();

        if(request.AccountTypeId.HasValue)
            query = query.Where(r=>r.AccountTypeId == request.AccountTypeId.Value);

        if(request.ParentId.HasValue)
            query = query.Where(r=>r.ParentId == request.ParentId.Value);

        var projection = query
            .OrderBy(o => o.DisplayOrder)
            .Select(r => new LedgerAccountDto
            {
                Id = r.Id,
                AccountTypeId = r.AccountTypeId,
                IsPostingAccount = r.IsPostingAccount,
                HasBeenUsedInEntries = r.HasBeenUsedInEntries,
                Name = r.Name,
                ParentId = r.ParentId
            });

        return await PaginatedList<LedgerAccountDto>.CreateAsync(projection, request.PageNumber,
                       request.PageSize, cancellationToken);
    }
}
