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

namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Queries.GetMoneyTransfersList;

public sealed class GetMoneyTransfersListQueryHandler
                        : IRequestHandler<GetMoneyTransfersListQuery, PaginatedList<MoneyTransferListItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserLookupService _userLookupService;
    private readonly ICurrentUserService _currentUser;

    public GetMoneyTransfersListQueryHandler(
        IApplicationDbContext context,
        IUserLookupService userLookupService,
        ICurrentUserService currentUser)
    {
        _context = context;
        _userLookupService = userLookupService;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<MoneyTransferListItemDto>> Handle(
                        GetMoneyTransfersListQuery request,
                        CancellationToken cancellationToken)
    {
        var query = _context.AccountingDocuments
            .AsNoTracking()
            .Where(d => d.DocumentType == DocumentType.MoneyTransfer);

        // ------------------------------------------------------------
        // Search
        // ------------------------------------------------------------

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var searchText = request.SearchText.Trim().Replace("%", "[%]").Replace("_", "[_]");

            query = query.Where(d =>
                (d.Description != null &&
                 EF.Functions.Like(d.Description, $"%{searchText}%"))

                ||

                d.Entries.Any(e =>
                    EF.Functions.Like(e.LedgerAccount.Name, $"%{searchText}%")));
        }

        // ------------------------------------------------------------
        // Date
        // ------------------------------------------------------------

        if (request.FromDate.HasValue)
        {
            query = query.Where(d =>
                d.DocumentDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(d =>
                d.DocumentDate <= request.ToDate.Value);
        }

        // ------------------------------------------------------------
        // Currency
        // ------------------------------------------------------------

        if (request.CurrencyId.HasValue)
        {
            query = query.Where(d =>
                d.CurrencyId == request.CurrencyId.Value);
        }

        // ------------------------------------------------------------
        // Amount
        // ------------------------------------------------------------

        if (request.FromAmount.HasValue)
        {
            query = query.Where(d =>
                d.Entries.Any(e =>
                    e.Debit > 0 &&
                    e.Debit >= request.FromAmount.Value));
        }

        if (request.ToAmount.HasValue)
        {
            query = query.Where(d =>
                d.Entries.Any(e =>
                    e.Debit > 0 &&
                    e.Debit <= request.ToAmount.Value));
        }

        // ------------------------------------------------------------
        // Ledger account
        // ------------------------------------------------------------

        if (request.LedgerAccountId.HasValue)
        {
            var ledgerAccountId = request.LedgerAccountId.Value;

            query = query.Where(d =>
                d.Entries.Any(e =>
                    e.LedgerAccountId == ledgerAccountId));
        }

        // ------------------------------------------------------------
        // Monetary account
        // ------------------------------------------------------------

        if (request.MonetaryAccountId.HasValue)
        {
            var monetaryAccountId = request.MonetaryAccountId.Value;

            query = query.Where(d =>
                d.Entries.Any(e =>
                    e.LedgerAccountId == _context.MonetaryAccounts
                        .Where(m => m.Id == monetaryAccountId)
                        .Select(m => m.LedgerAccountId)
                        .FirstOrDefault()));
        }

        // ------------------------------------------------------------
        // Person
        // ------------------------------------------------------------

        if (request.PersonId.HasValue)
        {
            var personId = request.PersonId.Value;

            query = query.Where(d =>
                d.Entries.Any(e =>
                    e.LedgerAccountId == _context.Persons
                        .Where(p => p.Id == personId)
                        .Select(p => p.LedgerAccountId)
                        .FirstOrDefault()));
        }

        // ------------------------------------------------------------
        // Projection
        // ------------------------------------------------------------

        var tenantUsers = await _userLookupService.GetTenantUsersAsync(_currentUser.TenantId, cancellationToken);
        var usersById = tenantUsers.ToDictionary(u => u.Id);

        var projected = query
            .OrderByDescending(d => d.DocumentDate)
            .ThenByDescending(d => d.CreatedAt)
            .Select(d => new MoneyTransferListItemDto
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
                CreatedByUserName = usersById.GetValueOrDefault(d.CreatedBy)!.FirstName + " " + usersById.GetValueOrDefault(d.CreatedBy)!.LastName

            });

        return await PaginatedList<MoneyTransferListItemDto>.CreateAsync(
            projected,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
