using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Common.Services;

public class AccountingLookupService : IAccountingLookupService
{
    private readonly IApplicationDbContext _context;

    public AccountingLookupService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Dictionary<Guid, LedgerAccount>> GetLedgerAccountsAsync(IEnumerable<Guid> ledgerAccountsIds,
                            CancellationToken cancellationToken)
    {
        var ids = ledgerAccountsIds.Distinct().ToList();

        return await _context.LedgerAccounts
                            .Where(a => ids.Contains(a.Id))
                            .ToDictionaryAsync(d => d.Id, cancellationToken);
    }

    public async Task<FundSourceLookup<MonetaryAccount>> GetMonetaryAccountsAsync(IEnumerable<Guid> monetaryAccountIds,
                            IEnumerable<Guid> alsoByLedgerAccountId, CancellationToken cancellationToken)
    {

        var ids = monetaryAccountIds.Distinct().ToList();
        var ledgerIds = alsoByLedgerAccountId.Distinct().ToList();

        var accounts = await _context.MonetaryAccounts
                            .Include(i => i.LedgerAccount)
                            .Where(r => ids.Contains(r.Id) || ledgerIds.Contains(r.LedgerAccountId))
                            .ToListAsync(cancellationToken);

        return new FundSourceLookup<MonetaryAccount>
        {
            ById = accounts.ToDictionary(a => a.Id),
            ByLedgerAccountId = accounts.ToDictionary(a => a.LedgerAccountId)
        };
    }

    public async Task<FundSourceLookup<Person>> GetPersonsAsync(IEnumerable<Guid> personsIds,
                            IEnumerable<Guid> alsoByLedgerAccountId, CancellationToken cancellationToken)
    {
        var ids = personsIds.Distinct().ToList();
        var ledgerIds = alsoByLedgerAccountId.Distinct().ToList();

        var accounts = await _context.Persons
                            .Include(i => i.LedgerAccount)
                            .Where(r => ids.Contains(r.Id) || ledgerIds.Contains(r.LedgerAccountId))
                            .ToListAsync(cancellationToken);

        return new FundSourceLookup<Person>
        {
            ById = accounts.ToDictionary(a => a.Id),
            ByLedgerAccountId = accounts.ToDictionary(a => a.LedgerAccountId)
        };
    }

}
