using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceApp.Domain.Common.Constants;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Infrastructure.Persistence.Configurations;

public class AccountingEntryConfiguration : IEntityTypeConfiguration<AccountingEntry>
{
    public void Configure(EntityTypeBuilder<AccountingEntry> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Debit).HasPrecision(DecimalPrecision.Precision, DecimalPrecision.MonetaryAmountScale);
        builder.Property(e => e.Credit).HasPrecision(DecimalPrecision.Precision, DecimalPrecision.MonetaryAmountScale);
        builder.Property(e => e.Description).HasMaxLength(FieldLengths.Description);


        // Restrict, not Cascade - deleting a LedgerAccount must never cascade-delete
        // historical entries. LedgerAccount.MarkAsUsed()/HasBeenUsedInEntries is the
        // domain-level guard that's supposed to prevent this scenario from arising at all.
        builder.HasOne(e => e.LedgerAccount)
            .WithMany()
            .HasForeignKey(e => e.LedgerAccountId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasIndex(e => new { e.TenantId,e.LedgerAccountId, e.IsDeleted });

    }
}
