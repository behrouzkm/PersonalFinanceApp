using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.Currencies.Queries.GetCurrenciesList;

public class GetCurrencyListQueryHandler : IRequestHandler<GetCurrenciesListQuery, PaginatedList<Currency>>
{
    private readonly IApplicationDbContext _context;

    public GetCurrencyListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<Currency>> Handle(GetCurrenciesListQuery request,
                        CancellationToken cancellationToken)
    {
        var query = _context.Currencies;

        return await PaginatedList<Currency>.CreateAsync(query, request.PageNumber,
                       request.PageSize, cancellationToken);
    }
}
