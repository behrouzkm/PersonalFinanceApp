using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Infrastructure.Persistence.Configurations;

// Configures the shared TPC (Table-Per-Concrete-type) base. No table of its own - see
// BankAccountConfiguration/CashAccountConfiguration, each of which owns its own full
// table including these shared columns. MonetaryAccount stays abstract; nothing is
// ever inserted as the base type, so it never needs a table.
public class MonetaryAccountConfiguration : IEntityTypeConfiguration<MonetaryAccount>
{
    public void Configure(EntityTypeBuilder<MonetaryAccount> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.HasKey(m => m.Id);

        builder.Property(m => m.DisplayName).IsRequired().HasMaxLength(200);
        builder.Property(m => m.InitialBalance).HasPrecision(18, 2);
        builder.Property(m => m.CurrentBalance).HasPrecision(18, 2);
        builder.Property(m => m.CreditLimit).HasPrecision(18, 2);

        builder.HasOne(m => m.LedgerAccount)
            .WithMany()
            .HasForeignKey(m => m.LedgerAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Currency)
            .WithMany()
            .HasForeignKey(m => m.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasIndex(m => m.LedgerAccountId).IsUnique();

    }
}
