using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<AccountingDocument> AccountingDocuments { get; }
    DbSet<AccountingEntry> AccountingEntries { get; }
    DbSet<LedgerAccount> LedgerAccounts { get; }
    DbSet<MonetaryAccount> MonetaryAccounts { get; }
    DbSet<BankAccount> BankAccounts { get; }
    DbSet<CashAccount> CashAccounts { get; }
    DbSet<AccountType> AccountTypes { get; }
    DbSet<AccountTypeTranslation> AccountTypeTranslations { get; }
    DbSet<Attachment> Attachments { get; }
    DbSet<Currency> Currencies { get; }
    DbSet<DocumentTypeTranslation> DocumentTypeTranslations { get; }
    DbSet<Language> Languages { get; }
    DbSet<MoneyTransfer> MoneyTransfers { get; }
    DbSet<Person> Persons { get; }
    DbSet<Tenant> Tenants { get; }
    DbSet<SystemTemplate> SystemTemplates { get; }



    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

}
