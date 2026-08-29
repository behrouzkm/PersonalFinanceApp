using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Common.Interfaces;

public interface IAccountingLookupService
{
    Task<Dictionary<Guid,LedgerAccount>> GetLedgerAccountsAsync(
            IEnumerable<Guid> ledgerAccountsIds, CancellationToken cancellationToken);


    Task<FundSourceLookup<MonetaryAccount>> GetMonetaryAccountsAsync(
        IEnumerable<Guid> monetaryAccountIds, IEnumerable<Guid> alsoByLedgerAccountId, CancellationToken cancellationToken);

    Task<FundSourceLookup<Person>> GetPersonsAsync(
          IEnumerable<Guid> PersonsIds, IEnumerable<Guid> alsoByLedgerAccountId, CancellationToken cancellationToken);

    Task<LedgerAccount?> GetOpeningBalanceEquityLedgerAccount(AccountCategory accountCategory,CancellationToken cancellationToken);
}
