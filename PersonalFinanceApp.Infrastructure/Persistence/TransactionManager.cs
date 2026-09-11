using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Common.Interfaces;

namespace PersonalFinanceApp.Infrastructure.Persistence;

public sealed class TransactionManager(ApplicationDbContext context) : ITransactionManager
{

    public async Task<IAppTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        var transaction = await context.Database
            .BeginTransactionAsync(cancellationToken);

        return new EfAppTransaction(transaction);
    }
}
