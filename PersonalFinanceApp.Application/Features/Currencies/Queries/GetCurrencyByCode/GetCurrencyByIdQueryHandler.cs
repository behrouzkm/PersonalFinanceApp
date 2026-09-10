using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Features.Currencies.Common;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.Currencies.Queries.GetCurrencyByCode;

public class GetCurrencyByIdQueryHandler : IRequestHandler<GetCurrencyByCodeQuery, CurrencyDto>
{
    private readonly IApplicationDbContext _context;

    public GetCurrencyByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CurrencyDto> Handle(GetCurrencyByCodeQuery request, CancellationToken cancellationToken)
    {
        var currency = await _context.Currencies
                .FirstOrDefaultAsync(d => d.Code == request.Code, cancellationToken)
            ?? throw new NotFoundException(nameof(Currency), request.Code);

        if(!currency.IsActive)
            throw new BusinessRuleException(ApplicationErrorCodes.Currency.CurrencyDeactivated);

        return new CurrencyDto
        {
            Id = currency.Id,
            Name = currency.Name,
            Code = currency.Code,
            IsActive = currency.IsActive,
            DecimalPlaces=currency.DecimalPlaces,
            Symbol=currency.Symbol
        };

    }
}
