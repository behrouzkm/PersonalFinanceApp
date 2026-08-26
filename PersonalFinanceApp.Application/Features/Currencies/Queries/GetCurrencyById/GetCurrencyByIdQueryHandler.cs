using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Features.Currencies.Common;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.Currencies.Queries.GetCurrencyById;

public class GetCurrencyByIdQueryHandler : IRequestHandler<GetCurrencyByIdQuery, CurrencyDto>
{
    private readonly IApplicationDbContext _context;

    public GetCurrencyByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CurrencyDto> Handle(GetCurrencyByIdQuery request, CancellationToken cancellationToken)
    {
        var currency = await _context.Currencies.AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Currency), request.Id);

        return new CurrencyDto
        {
            Id = currency.Id,
            Name = currency.Name,
            Code = currency.Code,
            IsActive = currency.IsActive,
            DecimalPlaces = currency.DecimalPlaces,
            Symbol = currency.Symbol
        };

    }
}
