using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.Persons.Common;


namespace PersonalFinanceApp.Application.Features.Persons.Queries.GetPersonsList;

public class GetPersonsListQueryHandler : IRequestHandler<GetPersonsListQuery, PaginatedList<PersonDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPersonsListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<PersonDto>> Handle(GetPersonsListQuery request,
                        CancellationToken cancellationToken)
    {
        var query = _context.Persons.Include(r=>r.Currency);

        var projection = query
            .OrderBy(o => o.DisplayOrder)
            .Select(r => new PersonDto
            {
                Id = r.Id,
                PersonType = r.PersonType,
                DisplayName = r.DisplayName,
                LedgerAccountId = r.LedgerAccountId,
                OpeningDate = r.OpeningDate,
                InitialBalance = r.InitialBalance,
                CurrentBalance = r.CurrentBalance,
                CreditLimit = r.CreditLimit,
                OpeningAccountingDocumentId = r.OpeningAccountingDocumentId,
                CurrencyId = r.CurrencyId,
                CurrencyName = r.Currency.Name,
                CurrencySymbol = r.Currency.Symbol,
                DisplayOrder = r.DisplayOrder,
                Email = r.Email,
                MobileNumber = r.MobileNumber,
                TelNumber = r.TelNumber
            });

        return await PaginatedList<PersonDto>.CreateAsync(projection, request.PageNumber,
                       request.PageSize, cancellationToken);
    }
}
