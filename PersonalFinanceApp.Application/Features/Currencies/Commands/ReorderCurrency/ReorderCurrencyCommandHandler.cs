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

namespace PersonalFinanceApp.Application.Features.Currencies.Commands.ReorderCurrency;

public class ReorderCurrencyCommandHandler : IRequestHandler<ReorderCurrencyCommand>
{
    private readonly IApplicationDbContext _context;

    public ReorderCurrencyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ReorderCurrencyCommand request, CancellationToken cancellationToken)
    {
        var currency = await _context.Currencies
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Currency), request.Id);


        var currenciesCount = await _context.Currencies
                .OrderBy(o => o.DisplayOrder)
                .CountAsync(cancellationToken);

        if (currenciesCount <= 1)
            return;

        var oldPosition = currency.DisplayOrder;

        var newPosition = Math.Clamp(request.NewDisplayOrder, 1, currenciesCount);

        if (oldPosition == newPosition)
            return;

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Move the selected currency temporarily outside the normal range.
            currency.SetDisplayOrder(currenciesCount + 1);
            await _context.SaveChangesAsync(cancellationToken);


            // 2. Shift the other currencies
            if (newPosition < oldPosition)
            {
                var inRangeCurrencies = await _context.Currencies
                    .Where(r => r.DisplayOrder >= oldPosition && r.DisplayOrder <= newPosition)
                    .ToListAsync(cancellationToken);

                foreach (var item in inRangeCurrencies)
                {
                    item.MoveUp();
                }
            }
            else
            {
                var inRangeCurrencies = await _context.Currencies
                    .Where(r => r.DisplayOrder >= newPosition && r.DisplayOrder <= oldPosition)
                    .ToListAsync(cancellationToken);

                foreach (var item in inRangeCurrencies)
                {
                    item.MoveDown();
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            // 3. Put the selected currency in its new position.
            currency.SetDisplayOrder(newPosition);
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
