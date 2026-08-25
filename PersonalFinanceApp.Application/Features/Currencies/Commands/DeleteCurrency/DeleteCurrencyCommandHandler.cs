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

namespace PersonalFinanceApp.Application.Features.Currencies.Commands.DeleteCurrency;

public class DeleteCurrencyCommandHandler : IRequestHandler<DeleteCurrencyCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteCurrencyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
    {
        var currency = await _context.Currencies
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Currency), request.Id);


        var currencyInUse = await _context.Tenants
                .AnyAsync(r => r.DefaultCurrencyId == request.Id, cancellationToken);

        currencyInUse |= await _context.MonetaryAccounts
                .AnyAsync(r => r.CurrencyId == request.Id, cancellationToken);

        currencyInUse |= await _context.Persons
                .AnyAsync(r => r.CurrencyId == request.Id, cancellationToken);

        currencyInUse |= await _context.AccountingDocuments
                .AnyAsync(r => r.CurrencyId == request.Id, cancellationToken);

        currencyInUse |= await _context.MoneyTransfers
                .AnyAsync(r => r.CurrencyId == request.Id, cancellationToken);

        if (currencyInUse)
            throw new BusinessRuleException(ApplicationErrorCodes.Currency.CurrencyInUse, request.Id, currency.Name);

        _context.Currencies.Remove(currency);

        await _context.SaveChangesAsync(cancellationToken);

    }
}
