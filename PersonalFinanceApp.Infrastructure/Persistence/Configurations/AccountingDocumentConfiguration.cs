using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Infrastructure.Persistence.Configurations;

public class AccountingDocumentConfiguration : IEntityTypeConfiguration<AccountingDocument>
{
    public void Configure(EntityTypeBuilder<AccountingDocument> builder)
    {
        builder.HasKey(d => d.Id);

        // [Flags] enum - store as int so bitwise queries work naturally
        builder.Property(d => d.DocumentType).HasConversion<int>().IsRequired();

        builder.Property(d=>d.DocumentDate).IsRequired();
        builder.Property(d =>d.CurrencyId).IsRequired();
        builder.Property(d =>d.Description).HasMaxLength(500);

        builder.Property(d =>d.RowVersion).IsRowVersion();

        // _entries is a private backing field behind the public Entries collection -
        // EF Core needs to be told explicitly to materialize through the field rather
        // than a (nonexistent) public setter.
        builder.HasMany(d=>d.Entries)
            .WithOne(e=>e.Document)
            .HasForeignKey(e=>e.AccountingDocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(d=>d.Entries)
            .HasField("_entries")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne(d=>d.Currency)
            .WithMany()
            .HasForeignKey(d=>d.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d=> new{d.TenantId,d.DocumentDate});
        builder.HasIndex(d=> new{d.TenantId,d.DocumentType});
    }
}
