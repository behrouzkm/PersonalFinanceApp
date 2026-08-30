using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Application.Common.Services;

public class ReorderService : IReorderService
{
    private readonly IApplicationDbContext _context;

    public ReorderService(IApplicationDbContext context) => _context = context;

    public async Task ReorderAsync<TEntity>(
        Expression<Func<TEntity, bool>> selector,
        object identifier,
        int newDisplayOrder,
        CancellationToken cancellationToken)
        where TEntity : class, IReorderable
    {
        var set = _context.Set<TEntity>();

        var entity = await set.FirstOrDefaultAsync(selector, cancellationToken)
            ?? throw new NotFoundException(typeof(TEntity).Name, identifier);

        var count = await set.CountAsync(cancellationToken);
        if (count <= 1)
            return;

        var oldPosition = entity.DisplayOrder;
        var newPosition = Math.Clamp(newDisplayOrder, 1, count);

        if (oldPosition == newPosition)
            return;

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // 1. Move the target outside the valid range — required whenever the
            // entity has a unique DisplayOrder index (Currency, Language), and
            // harmless when it doesn't (Person, today).
            entity.SetDisplayOrder(count + 1);
            await _context.SaveChangesAsync(cancellationToken);

            // 2. Shift everything between the old and new position.
            if (newPosition < oldPosition)
            {
                var inRange = await set
                    .Where(e => e.DisplayOrder >= newPosition && e.DisplayOrder < oldPosition)
                    .ToListAsync(cancellationToken);

                foreach (var item in inRange)
                    item.IncrementDisplayOrder();
            }
            else
            {
                var inRange = await set
                    .Where(e => e.DisplayOrder > oldPosition && e.DisplayOrder <= newPosition)
                    .ToListAsync(cancellationToken);

                foreach (var item in inRange)
                    item.DecrementDisplayOrder();
            }

            await _context.SaveChangesAsync(cancellationToken);

            // 3. Drop the target into its final position.
            entity.SetDisplayOrder(newPosition);
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
