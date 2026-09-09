using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Application.Common.Interfaces;

public interface IReorderService
{
    Task ReorderAsync<TEntity>(
        Expression<Func<TEntity, bool>> selector,
        object identifier,          // used only for the NotFoundException message
        int newDisplayOrder,
        CancellationToken cancellationToken,
        Expression<Func<TEntity, bool>>? scope = null)
        where TEntity : class, IReorderable;


    /// oldGroupScope/newGroupScope must describe "the OTHER siblings in this group" —
    /// i.e. they must exclude the entity being moved (e.g. `l => l.ParentId == x && l.Id != movedId`).
    /// The service can't enforce this generically since IReorderable has no key accessor.
    Task MoveBetweenGroupsAsync<TEntity>(
        Expression<Func<TEntity, bool>> selector,
        object identifier,
        Expression<Func<TEntity, bool>> oldGroupScope,
        Expression<Func<TEntity, bool>> newGroupScope,
        Action<TEntity> assignToNewGroup,
        int newDisplayOrder,
        CancellationToken cancellationToken)
        where TEntity : class, IReorderable;

    /// Call right before soft-deleting an IReorderable entity — closes the gap
    /// it will leave, so the surviving active set stays contiguous.
    Task CloseGapAsync<TEntity>(
        TEntity entity, CancellationToken cancellationToken, Expression<Func<TEntity, bool>>? scope)
        where TEntity : class, IReorderable;

    /// Call right after restoring a soft-deleted IReorderable entity — its old
    /// DisplayOrder may already belong to something else post-compaction, so it
    /// always re-enters at the end rather than reclaiming its former position.
    Task AppendToEndAsync<TEntity>(
        TEntity entity, Expression<Func<TEntity, bool>> scope, CancellationToken cancellationToken)
        where TEntity : class, IReorderable;
}
