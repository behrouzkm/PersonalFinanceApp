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

public sealed class GetMoneyTransfersListQueryHandler(IApplicationDbContext _context)
                        : IRequestHandler<GetMoneyTransfersListQuery, PaginatedList<MoneyTransferListItemDto>>
{

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
            var searchText = request.SearchText.Trim();

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

        var projected = query
            .OrderByDescending(d => d.DocumentDate)
            .ThenByDescending(d => d.CreatedAt)
            .Select(d => new MoneyTransferListItemDto
            {
                AccountingDocumentId = d.Id,

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
                    .Count(a => a.AccountingDocumentId == d.Id)
            });

        return await PaginatedList<MoneyTransferListItemDto>.CreateAsync(
            projected,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}

// public class GetMoneyTransfersListQueryHandler : IRequestHandler<GetMoneyTransfersListQuery, PaginatedList<MoneyTransferListItemDto>>
// {
//     private readonly IApplicationDbContext _context;

//     public GetMoneyTransfersListQueryHandler(IApplicationDbContext context)
//     {
//         _context = context;
//     }

//     public async Task<PaginatedList<MoneyTransferListItemDto>> Handle(GetMoneyTransfersListQuery request,
//                         CancellationToken cancellationToken)
//     {
//         var query = _context.AccountingDocuments
//             .Include(c => c.Currency)
//             .Where(d => d.DocumentType == DocumentType.MoneyTransfer);

//         if (!string.IsNullOrWhiteSpace(request.SearchText))
//         {
//             var text = request.SearchText.Trim().ToLowerInvariant();
//             var monetaryAccounts = await _context.MonetaryAccounts
//                     .Where(m => m.DisplayName.ToLowerInvariant().Contains(text))
//                     .Select(s => s.LedgerAccountId)
//                     .ToListAsync(cancellationToken);

//             var persons = await _context.Persons
//                     .Where(m => m.DisplayName.ToLowerInvariant().Contains(text))
//                     .Select(s => s.LedgerAccountId)
//                     .ToListAsync(cancellationToken);

//             query = query.Where(r => (r.Description != null && r.Description.ToLowerInvariant().Contains(text)) ||
//                 (monetaryAccounts.Any() && r.Entries.Any(e => monetaryAccounts.Contains(e.LedgerAccountId))) ||
//                 (persons.Any() && r.Entries.Any(e => persons.Contains(e.LedgerAccountId))));
//         }



//         if (request.FromDate.HasValue)
//             query = query.Where(d => d.DocumentDate >= request.FromDate);

//         if (request.ToDate.HasValue)
//             query = query.Where(d => d.DocumentDate <= request.ToDate);

//         if (request.CurrencyId.HasValue)
//             query = query.Where(r => r.CurrencyId == request.CurrencyId.Value);

//         if (request.FromAmount.HasValue)
//             query = query.Where(r => r.Entries.Any(e => e.Debit >= request.FromAmount.Value || e.Credit >= request.FromAmount.Value));

//         if (request.ToAmount.HasValue)
//             query = query.Where(r => r.Entries.Any(e => e.Debit <= request.ToAmount.Value || e.Credit <= request.ToAmount.Value));

//         if (request.LedgerAccountId.HasValue)
//             query = query.Where(d => d.Entries.Any(e => e.LedgerAccountId == request.LedgerAccountId.Value));

//         if (request.MonetaryAccountId.HasValue)
//         {
//             var ledgerAccountId = await _context.MonetaryAccounts
//                 .Where(m => m.Id == request.MonetaryAccountId.Value)
//                 .Select(m => (Guid?)m.LedgerAccountId)
//                 .FirstOrDefaultAsync(cancellationToken);

//             query = query.Where(d => d.Entries.Any(e => e.LedgerAccountId == ledgerAccountId));
//         }

//         if (request.PersonId.HasValue)
//         {
//             var ledgerAccountId = await _context.Persons
//                 .Where(p => p.Id == request.PersonId.Value)
//                 .Select(p => (Guid?)p.LedgerAccountId)
//                 .FirstOrDefaultAsync(cancellationToken);

//             query = query.Where(d => d.Entries.Any(e => e.LedgerAccountId == ledgerAccountId));
//         }
//         var ledgerAccountTypes = await _context.AccountTypes
//             .Where(t => t.Category == AccountCategory.PersonAccount || t.Category == AccountCategory.BankAccount || t.Category == AccountCategory.CashAccount)
//             .Select(t => t.Id)
//             .ToListAsync(cancellationToken);

//         var ledgerAccountNames = await _context.LedgerAccounts
//             .Where(l => ledgerAccountTypes.Contains(l.AccountTypeId))
//             .ToDictionaryAsync(l => l.Id, l => l.Name, cancellationToken);

//         var projected = query
//             .OrderByDescending(d => d.DocumentDate)
//             .ThenByDescending(d => d.CreatedAt)
//             .Select(d => new MoneyTransferListItemDto
//             {
//                 AccountingDocumentId = d.Id,
//                 TransferDate = d.DocumentDate,
//                 CurrencyId = d.CurrencyId,
//                 CurrencySymbol = d.Currency.Symbol,
//                 Description = d.Description,
//                 Amount = d.Entries.First(r => r.Debit > 0).Debit,
//                 FromLedgerAccountId = d.Entries.First(r => r.Credit > 0).LedgerAccountId,
//                 ToLedgerAccountId = d.Entries.First(r => r.Debit > 0).LedgerAccountId,
//                 FromAccountName = ledgerAccountNames[d.Entries.First(r => r.Credit > 0).LedgerAccountId],
//                 ToAccountName = ledgerAccountNames[d.Entries.First(r => r.Debit > 0).LedgerAccountId],
//                 AttachmentCount = _context.Attachments.Count(a => a.AccountingDocumentId == d.Id)
//             });

//         return await PaginatedList<MoneyTransferListItemDto>.CreateAsync(projected, request.PageNumber,
//                         request.PageSize, cancellationToken);
//     }
// }

