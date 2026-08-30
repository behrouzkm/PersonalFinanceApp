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
        CancellationToken cancellationToken)
        where TEntity : class, IReorderable;
}
