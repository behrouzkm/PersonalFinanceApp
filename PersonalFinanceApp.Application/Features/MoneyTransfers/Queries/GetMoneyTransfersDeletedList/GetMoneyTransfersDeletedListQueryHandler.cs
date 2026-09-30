using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.MoneyTransfers.Common;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Queries.GetMoneyTransfersDeletedList;

public sealed class GetMoneyTransfersDeletedListQueryHandler
                        : IRequestHandler<GetMoneyTransfersDeletedListQuery, PaginatedList<MoneyTransferDeletedListItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserLookupService _userLookupService;
    private readonly ICurrentUserService _currentUser;

    public GetMoneyTransfersDeletedListQueryHandler(
        IApplicationDbContext context,
        IUserLookupService userLookupService,
        ICurrentUserService currentUser)
    {
        _context = context;
        _userLookupService = userLookupService;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<MoneyTransferDeletedListItemDto>> Handle(
                        GetMoneyTransfersDeletedListQuery request,
                        CancellationToken cancellationToken)
    {
        var query = _context.AccountingDocuments
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(d => d.TenantId == _currentUser.TenantId &&
                        d.IsDeleted == true &
                        d.DocumentType == DocumentType.MoneyTransfer);



        // ------------------------------------------------------------
        // Projection
        // ------------------------------------------------------------

        var tenantUsers = await _userLookupService.GetTenantUsersAsync(_currentUser.TenantId, cancellationToken);
        var usersById = tenantUsers.ToDictionary(u => u.Id);

        var projected = query
            .OrderByDescending(d => d.DocumentDate)
            .ThenByDescending(d => d.CreatedAt)
            .Select(d => new MoneyTransferDeletedListItemDto
            {
                MoneyTransferDocumentId = d.Id,

                TransferDate = d.DocumentDate,

                CurrencyId = d.CurrencyId,

                CurrencySymbol = d.Currency.Symbol,

                CurrencyDecimalPlaces = d.Currency.DecimalPlaces,

                Description = d.Description,

                Amount = d.Entries
                    .Where(e => e.Debit > 0)
                    .Select(e => e.Debit)
                    .FirstOrDefault(),

                FromLedgerAccountId = d.Entries
                    .Where(e => e.Credit > 0)
                    .Select(e => (Guid?)e.LedgerAccountId)
                    .FirstOrDefault(),

                ToLedgerAccountId = d.Entries
                    .Where(e => e.Debit > 0)
                    .Select(e => (Guid?)e.LedgerAccountId)
                    .FirstOrDefault(),

                FromAccountName = d.Entries
                    .Where(e => e.Credit > 0)
                    .Select(e => e.LedgerAccount.Name)
                    .FirstOrDefault() ?? string.Empty,

                ToAccountName = d.Entries
                    .Where(e => e.Debit > 0)
                    .Select(e => e.LedgerAccount.Name)
                    .FirstOrDefault() ?? string.Empty,

                AttachmentCount = _context.Attachments
                    .Count(a => a.AccountingDocumentId == d.Id),

                CreatedAt = d.CreatedAt,
                CreatedBy = d.CreatedBy,
                CreatedByUserName = usersById.GetValueOrDefault(d.CreatedBy)!.FirstName + " " + usersById.GetValueOrDefault(d.CreatedBy)!.LastName,

                DeletedAt = d.DeletedAt!.Value,
                DeletedBy = d.DeletedBy!.Value,
                DeletedByUserName = usersById.GetValueOrDefault(d.DeletedBy.Value)!.FirstName + " " + usersById.GetValueOrDefault(d.DeletedBy.Value)!.LastName

            });

        return await PaginatedList<MoneyTransferDeletedListItemDto>.CreateAsync(
            projected,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
