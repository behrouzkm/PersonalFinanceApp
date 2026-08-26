using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.Currencies.Commands.CreateCurrency;

public class CreateCurrencyCommandHandler : IRequestHandler<CreateCurrencyCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateCurrencyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var duplicateExist = await _context.Currencies
                .AnyAsync(c => c.Code == request.Code || c.Name == request.Name, cancellationToken);

        if (duplicateExist)
            throw new BusinessRuleException(ApplicationErrorCodes.Currency.DuplicateCodeOrName, request.Code, request.Name);

        var maxDisplayOrder = await _context.Currencies.MaxAsync(c => (int?)c.DisplayOrder, cancellationToken) ?? 0;

        var currency = new Currency(
             request.Code,
            request.Name,
            request.IsActive,
            maxDisplayOrder + 1,
            request.DecimalPlaces,
            request.Symbol);

        await _context.Currencies.AddAsync(currency, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return currency.Id;

    }
}
