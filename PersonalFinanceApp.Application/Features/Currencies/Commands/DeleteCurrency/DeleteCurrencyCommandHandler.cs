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
    private readonly IReorderService _reorderService;

    public DeleteCurrencyCommandHandler(IApplicationDbContext context, IReorderService reorderService)
    {
        _context = context;
        _reorderService = reorderService;
    }

    public async Task Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
    {
        var currency = await _context.Currencies
              .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
              ?? throw new NotFoundException(nameof(Currency), request.Id);

        var currencyInUse = await _context.Tenants.AnyAsync(r => r.DefaultCurrencyId == request.Id, cancellationToken)
            || await _context.MonetaryAccounts.AnyAsync(r => r.CurrencyId == request.Id, cancellationToken)
            || await _context.Persons.AnyAsync(r => r.CurrencyId == request.Id, cancellationToken)
            || await _context.AccountingDocuments.AnyAsync(r => r.CurrencyId == request.Id, cancellationToken);

        if (currencyInUse)
            throw new BusinessRuleException(ApplicationErrorCodes.Currency.CurrencyInUse, request.Id, currency.Name);

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Phase 1: the row must actually be gone before anything shifts into its slot.
            _context.Currencies.Remove(currency);
            await _context.SaveChangesAsync(cancellationToken);

            // Phase 2: now the slot is genuinely vacant.
            await _reorderService.CloseGapAsync(currency, cancellationToken, null);
            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
