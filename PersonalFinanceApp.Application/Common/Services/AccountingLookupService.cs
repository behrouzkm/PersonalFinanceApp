using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Application.Common.Services;

public class AccountingLookupService : IAccountingLookupService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public AccountingLookupService(
        IApplicationDbContext context,
        ICurrentUserService userService,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _userService = userService;
        _unitOfWork =unitOfWork;
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

    public async Task<LedgerAccount?> GetOpeningBalanceEquityLedgerAccount(AccountCategory accountCategory, CancellationToken cancellationToken)
    {
        var accountType = await _context.AccountTypes
            .FirstOrDefaultAsync(r => r.Category == accountCategory, cancellationToken);

        if (accountType != null)
        {
            var ledgerAccount = await _context.LedgerAccounts
                .FirstOrDefaultAsync(r => r.AccountTypeId == accountType.Id &&
                    r.ParentId == null, cancellationToken);

            return ledgerAccount;
        }

        return null;
    }

    public async Task<(IFundSource FundSource, LedgerAccount LedgerAccount)> GetFundSourceByLedgerAccountIdAsync(Guid ledgerAccountId, CancellationToken cancellationToken)
    {
        var ledgerAccount = await _context.LedgerAccounts
                .Include(l => l.AccountType)
                .FirstOrDefaultAsync(r => r.Id == ledgerAccountId, cancellationToken)
                ?? throw new NotFoundException(nameof(LedgerAccount), ledgerAccountId);

        if (ledgerAccount.AccountType.Category == AccountCategory.PersonAccount)
        {
            var personAccount = await _context.Persons
                    .FirstOrDefaultAsync(p => p.LedgerAccountId == ledgerAccountId, cancellationToken)
                    ?? throw new NotFoundException(nameof(Person), ledgerAccountId);

            return (personAccount, ledgerAccount);
        }
        else if (ledgerAccount.AccountType.Category == AccountCategory.BankAccount ||
                    ledgerAccount.AccountType.Category == AccountCategory.CashAccount)
        {
            var monetaryAccount = await _context.MonetaryAccounts
                    .FirstOrDefaultAsync(r => r.LedgerAccountId == ledgerAccountId, cancellationToken)
                    ?? throw new NotFoundException(nameof(MonetaryAccount), ledgerAccountId);

            return (monetaryAccount, ledgerAccount);
        }

        throw new NotFoundException(nameof(IFundSource), ledgerAccountId);
    }

    public async Task<LedgerAccount> GetOrCreateCurrencyExchangeClearingLedgerAccountAsync(int currencyId, CancellationToken cancellationToken)
    {
        var ledgerAccount = await _context.LedgerAccounts
                .FirstOrDefaultAsync(r => r.CurrencyId == currencyId, cancellationToken);

        if (ledgerAccount is null)
        {
            var accountType = await _context.AccountTypes
                .FirstOrDefaultAsync(r => r.Category == AccountCategory.CurrencyExchangeClearing, cancellationToken)
                ?? throw new NotFoundException(nameof(AccountType), AccountCategory.CurrencyExchangeClearing);

            var currency = await _context.Currencies
                .FirstOrDefaultAsync(r => r.Id == currencyId, cancellationToken)
                ?? throw new NotFoundException(nameof(Currency), currencyId);

            ledgerAccount = new LedgerAccount(accountType.Id, currency.Name, _userService.TenantId, _userService.UserId);

            _context.LedgerAccounts.Add(ledgerAccount);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return ledgerAccount;
    }
}
