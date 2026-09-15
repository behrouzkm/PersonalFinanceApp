using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.CashAccounts.Common;


namespace PersonalFinanceApp.Application.Features.CashAccounts.Queries.GetCashAccountsList;

public class GetCashAccountsListQueryHandler : IRequestHandler<GetCashAccountsListQuery, PaginatedList<CashAccountListItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAttachmentService _attachmentService;

    public GetCashAccountsListQueryHandler(IApplicationDbContext context, IAttachmentService attachmentService)
    {
        _context = context;
        _attachmentService = attachmentService;
    }

    public async Task<PaginatedList<CashAccountListItemDto>> Handle(GetCashAccountsListQuery request,
                        CancellationToken cancellationToken)
    {
        var query = _context.CashAccounts.Include(r => r.Currency).AsQueryable();


        if (request.CurrencyId.HasValue)
            query = query.Where(r => r.CurrencyId == request.CurrencyId.Value);

        var projection = query
            .OrderBy(o => o.DisplayOrder)
            .Select(r => new CashAccountListItemDto
            {
                Id = r.Id,
                DisplayName = r.DisplayName,
                LedgerAccountId = r.LedgerAccountId,
                OpeningDate = r.OpeningDate,
                InitialBalance = r.InitialBalance,
                CurrentBalance = r.CurrentBalance,
                OpeningAccountingDocumentId = r.OpeningAccountingDocumentId,
                CurrencyId = r.CurrencyId,
                CurrencyName = r.Currency.Name,
                CurrencySymbol = r.Currency.Symbol,
                DisplayOrder = r.DisplayOrder,
                Location = r.Location,
                IsPhysical = r.IsPhysical,
                AttachmentCount = _context.Attachments.Count(a => a.MonetaryAccountId == r.Id)
            });

        return await PaginatedList<CashAccountListItemDto>.CreateAsync(projection, request.PageNumber,
                       request.PageSize, cancellationToken);
    }
}
