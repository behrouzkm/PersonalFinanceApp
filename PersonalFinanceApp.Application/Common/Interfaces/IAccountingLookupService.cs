using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;
using PersonalFinanceApp.Domain.Interfaces;

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

    // Resolves the concrete Person/MonetaryAccount owning this ledger account, plus the
    // LedgerAccount itself (avoids a second round trip for MarkAsUsed()). Throws
    // NotFoundException if the ledger account doesn't belong to any fund source.
    Task<(IFundSource FundSource, LedgerAccount LedgerAccount)> GetFundSourceByLedgerAccountIdAsync(
        Guid ledgerAccountId, CancellationToken cancellationToken);

    // Lazily creates the per-currency clearing leaf under the CurrencyExchangeClearing
    // root the first time a tenant exchanges that currency — mirrors
    // GetOpeningBalanceEquityLedgerAccount's role for opening balances.
    Task<LedgerAccount> GetOrCreateCurrencyExchangeClearingLedgerAccountAsync(
        int currencyId, CancellationToken cancellationToken);
}
