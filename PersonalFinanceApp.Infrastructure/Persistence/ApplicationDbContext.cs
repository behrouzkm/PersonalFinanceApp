using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Common;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUser;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUser)
        : base(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<AccountingDocument> AccountingDocuments => Set<AccountingDocument>();
    public DbSet<AccountingEntry> AccountingEntries => Set<AccountingEntry>();
    public DbSet<LedgerAccount> LedgerAccounts => Set<LedgerAccount>();

    public DbSet<MonetaryAccount> MonetaryAccounts => Set<MonetaryAccount>();
    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<CashAccount> CashAccounts => Set<CashAccount>();

    public DbSet<AccountType> AccountTypes => Set<AccountType>();
    public DbSet<AccountTypeTranslation> AccountTypeTranslations => Set<AccountTypeTranslation>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<DocumentTypeTranslation> DocumentTypeTranslations => Set<DocumentTypeTranslation>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<MoneyTransfer> MoneyTransfers => Set<MoneyTransfer>();
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<SystemTemplate> SystemTemplates => Set<SystemTemplate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global query filter: every entity derived from BaseAuditableEntity is
        // automatically scoped to the current tenant and excludes soft-deleted rows,
        // on every query - including through .Include() navigations. This is applied
        // by reflection once here rather than repeated by hand per entity, so a newly
        // added entity gets it for free instead of relying on someone remembering to
        // wire it up. Use .IgnoreQueryFilters() at the one deliberate call site that
        // genuinely needs to bypass this (e.g. RestoreExpenditureCommandHandler).
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(BaseAuditableEntity).IsAssignableFrom(entityType.ClrType))
                continue;

            var type = entityType as BaseEntity;
            if (type == null)
                continue;

            var parameter = Expression.Parameter(entityType.ClrType, "e");

            var tenantIdProperty = Expression.Property(parameter, nameof(BaseEntity.TenantId));
            var currentTenantId = Expression.Property(Expression.Constant(_currentUser), nameof(ICurrentUserService.TenantId));
            var tenantCheck = Expression.Equal(tenantIdProperty, currentTenantId);

            var isDeletedProperty = Expression.Property(parameter, nameof(BaseAuditableEntity.IsDeleted));
            var notDeletedCheck = Expression.Equal(isDeletedProperty, Expression.Constant(false));

            var combined = Expression.AndAlso(tenantCheck, notDeletedCheck);
            var lambda = Expression.Lambda(combined, parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }

        base.OnModelCreating(modelBuilder);
    }

    // Entry<TEntity>(TEntity) is not redeclared here - DbContext already exposes a
    // public method with this exact signature, which satisfies IApplicationDbContext
    // implicitly. No override or `new` needed.
}
