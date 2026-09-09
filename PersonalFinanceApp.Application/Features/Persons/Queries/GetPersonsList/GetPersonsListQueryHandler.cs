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
        var query = _context.Persons.Include(r=>r.Currency);

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
                DisplayOrder = p.DisplayOrder,
                Email = p.Email,
                MobileNumber = p.MobileNumber,
                TelNumber = p.TelNumber,
                AttachmentCount = _context.Attachments.Count(a =>  a.PersonId == p.Id)
            });

        return await PaginatedList<PersonListItemDto>.CreateAsync(projection, request.PageNumber,
                       request.PageSize, cancellationToken);
    }
}
