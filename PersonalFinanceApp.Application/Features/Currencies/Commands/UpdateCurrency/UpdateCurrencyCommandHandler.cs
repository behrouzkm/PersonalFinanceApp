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

namespace PersonalFinanceApp.Application.Features.Currencies.Commands.UpdateCurrency;

public class UpdateCurrencyCommandHandler : IRequestHandler<UpdateCurrencyCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateCurrencyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var currency = await _context.Currencies
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Currency), request.Id);

        var duplicateExist = await _context.Currencies
                .AnyAsync(
                    c => c.Id != request.Id
                    && (c.Code == request.Code || c.Name == request.Name), cancellationToken);

        if (duplicateExist)
            throw new BusinessRuleException(ApplicationErrorCodes.Currency.DuplicateCodeOrName, request.Code, request.Name);


        currency.UpdateCurrency(request.Code,request.Name,request.IsActive, request.DecimalPlaces,request.Symbol);

        await _context.SaveChangesAsync(cancellationToken);

    }
}
