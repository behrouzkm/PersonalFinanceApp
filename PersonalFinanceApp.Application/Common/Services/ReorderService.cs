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
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransactionManager _transactionManager;

    public ReorderService(
            IApplicationDbContext context,
            IUnitOfWork unitOfWork,
            ITransactionManager transactionManager)
            {
                _context = context;
                _unitOfWork= unitOfWork;
                _transactionManager=transactionManager;
            }

    public async Task ReorderAsync<TEntity>(
    Expression<Func<TEntity, bool>> selector, object identifier, int newDisplayOrder,
    CancellationToken cancellationToken, Expression<Func<TEntity, bool>>? scope = null)
    where TEntity : class, IReorderable
    {
        var set = _context.Set<TEntity>();
        var scopedSet = scope is null ? set : set.Where(scope);

        var oldPosition = await set.Where(selector).Select(e => (int?)e.DisplayOrder)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(typeof(TEntity).Name, identifier);

        var count = await scopedSet.CountAsync(cancellationToken);
        if (count <= 1) return;

        var newPosition = Math.Clamp(newDisplayOrder, 1, count);
        if (oldPosition == newPosition) return;

        await using var transaction = await _transactionManager.BeginTransactionAsync(cancellationToken);
        try
        {
            // 1. Park the target outside the valid range. count+1 can never fall inside
            // either shift range below (both bounded within [1, count]), so it's safe
            // regardless of statement order — no self-exclusion needed anywhere here.
            await set.Where(selector)
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.DisplayOrder, count + 1), cancellationToken);

            // 2. Shift the range — one set-based statement, evaluated against final
            // state only, so no transient collision regardless of row order.
            if (newPosition < oldPosition)
            {
                await scopedSet.Where(e => e.DisplayOrder >= newPosition && e.DisplayOrder < oldPosition)
                    .ExecuteUpdateAsync(s => s.SetProperty(e => e.DisplayOrder, e => e.DisplayOrder + 1), cancellationToken);
            }
            else
            {
                await scopedSet.Where(e => e.DisplayOrder > oldPosition && e.DisplayOrder <= newPosition)
                    .ExecuteUpdateAsync(s => s.SetProperty(e => e.DisplayOrder, e => e.DisplayOrder - 1), cancellationToken);
            }

            // 3. Drop the target into its final position.
            await set.Where(selector)
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.DisplayOrder, newPosition), cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }


    public async Task MoveBetweenGroupsAsync<TEntity>(
    Expression<Func<TEntity, bool>> selector, object identifier,
    Expression<Func<TEntity, bool>> oldGroupScope, Expression<Func<TEntity, bool>> newGroupScope,
    Action<TEntity> assignToNewGroup, int newDisplayOrder, CancellationToken cancellationToken)
    where TEntity : class, IReorderable
    {
        var set = _context.Set<TEntity>();
        var entity = await set.FirstOrDefaultAsync(selector, cancellationToken)
            ?? throw new NotFoundException(typeof(TEntity).Name, identifier);

        var oldGroup = set.Where(oldGroupScope);   // caller-excluded: never matches `entity`
        var newGroup = set.Where(newGroupScope);   // caller-excluded: never matches `entity`

        var oldPosition = entity.DisplayOrder;
        var oldGroupCount = await oldGroup.CountAsync(cancellationToken);
        var newGroupCount = await newGroup.CountAsync(cancellationToken);
        var targetPosition = Math.Clamp(newDisplayOrder, 1, newGroupCount + 1);

        await using var transaction = await _transactionManager.BeginTransactionAsync(cancellationToken);
        try
        {
            // 1. Park the target — tracked path, since RowVersion (if present) must stay
            // in sync with what the final SaveChangesAsync below will check against.
            entity.SetDisplayOrder(Math.Max(oldGroupCount, newGroupCount) + 1);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 2. Close the gap in the old group — set-based, and since oldGroupScope
            // already excludes the entity, its parked row is never touched here.
            await oldGroup.Where(e => e.DisplayOrder > oldPosition)
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.DisplayOrder, e => e.DisplayOrder - 1), cancellationToken);

            // 3. Open a gap in the new group — entity still has the OLD ParentId at
            // this point, so newGroupScope wouldn't match it even without the exclusion,
            // but requiring it from callers on both scopes keeps the contract uniform.
            await newGroup.Where(e => e.DisplayOrder >= targetPosition)
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.DisplayOrder, e => e.DisplayOrder + 1), cancellationToken);

            // 4. Reassign the group and drop into the vacant slot — tracked path,
            // this is where domain validation (e.g. ChangeParent) actually runs.
            assignToNewGroup(entity);
            entity.SetDisplayOrder(targetPosition);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }


    public async Task CloseGapAsync<TEntity>(
        TEntity entity, CancellationToken cancellationToken, Expression<Func<TEntity, bool>>? scope = null)
        where TEntity : class, IReorderable
    {
        var deletedPosition = entity.DisplayOrder;
        var set = _context.Set<TEntity>();
        var scopedSet = scope is null ? set : set.Where(scope);

        await scopedSet.Where(e => e.DisplayOrder > deletedPosition)
            .ExecuteUpdateAsync(s => s.SetProperty(e => e.DisplayOrder, e => e.DisplayOrder - 1), cancellationToken);
    }

    public async Task AppendToEndAsync<TEntity>(
        TEntity entity, Expression<Func<TEntity, bool>> scope, CancellationToken cancellationToken)
        where TEntity : class, IReorderable
    {
        var count = await _context.Set<TEntity>().Where(scope).CountAsync(cancellationToken);
        entity.SetDisplayOrder(count + 1);
    }


}
