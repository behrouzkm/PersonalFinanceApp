using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.Persons.Common;


namespace PersonalFinanceApp.Application.Features.Persons.Queries.GetPersonsList;

public class GetPersonsListQueryHandler : IRequestHandler<GetPersonsListQuery, PaginatedList<PersonListItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPersonsListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<PersonListItemDto>> Handle(GetPersonsListQuery request,
                        CancellationToken cancellationToken)
    {
        var query = _context.Persons
                .Include(r => r.Currency)
                .AsNoTracking();

        // ------------------------------------------------------------
        // Search
        // ------------------------------------------------------------

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var searchText = request.SearchText.Trim().Replace("%", "[%]").Replace("_", "[_]");

            query = query.Where(d =>
                EF.Functions.Like(d.DisplayName, $"%{searchText}%")

                ||

                (d.Description != null &&
                 EF.Functions.Like(d.Description, $"%{searchText}%"))

                ||

                (d.Email != null &&
                 EF.Functions.Like(d.Email, $"%{searchText}%")));
        }

        // ------------------------------------------------------------
        // PersonType
        // ------------------------------------------------------------

        if (request.PersonType.HasValue)
        {
            query = query.Where(d =>
                d.PersonType == request.PersonType.Value);
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
        // PhoneNumber
        // ------------------------------------------------------------

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var phoneNumber = request.PhoneNumber.Trim().Replace("%", "[%]").Replace("_", "[_]");

            query = query.Where(d => (d.MobileNumber != null &&
                 EF.Functions.Like(d.MobileNumber, $"%{phoneNumber}%"))

                ||

                (d.TelNumber != null &&
                 EF.Functions.Like(d.TelNumber, $"%{phoneNumber}%")));
        }

        // ------------------------------------------------------------
        // Balance
        // ------------------------------------------------------------

        if (request.FromBalance.HasValue)
        {
            query = query.Where(d =>
                d.CurrentBalance >= request.FromBalance.Value);
        }

        if (request.ToBalance.HasValue)
        {
            query = query.Where(d =>
                d.CurrentBalance <= request.ToBalance.Value);
        }

        // ------------------------------------------------------------
        // Projection
        // ------------------------------------------------------------

        var projection = query
            .OrderBy(o => o.DisplayOrder)
            .Select(p => new PersonListItemDto
            {
                Id = p.Id,
                PersonType = p.PersonType,
                DisplayName = p.DisplayName,
                LedgerAccountId = p.LedgerAccountId,
                OpeningDate = p.OpeningDate,
                InitialBalance = p.InitialBalance,
                CurrentBalance = p.CurrentBalance,
                CreditLimit = p.CreditLimit,
                OpeningAccountingDocumentId = p.OpeningAccountingDocumentId,
                CurrencyId = p.CurrencyId,
                CurrencyName = p.Currency.Name,
                CurrencySymbol = p.Currency.Symbol,
                CurrencyDecimalPlaces = p.Currency.DecimalPlaces,
                DisplayOrder = p.DisplayOrder,
                Email = p.Email,
                MobileNumber = p.MobileNumber,
                TelNumber = p.TelNumber,
                AttachmentCount = _context.Attachments.Count(a => a.PersonId == p.Id)
            });

        return await PaginatedList<PersonListItemDto>.CreateAsync(projection, request.PageNumber,
                       request.PageSize, cancellationToken);
    }
}
