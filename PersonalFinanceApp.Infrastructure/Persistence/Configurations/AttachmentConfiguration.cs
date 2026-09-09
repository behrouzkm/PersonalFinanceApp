using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceApp.Domain.Common.Constants;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Infrastructure.Persistence.Configurations;

public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.Property(a => a.FileName).HasMaxLength(FieldLengths.Name);
        builder.Property(a => a.ContentType).HasMaxLength(100);
        builder.Property(a => a.StorageKey).HasMaxLength(500);

        builder.HasOne(a => a.AccountingDocument).WithMany()
            .HasForeignKey(a => a.AccountingDocumentId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Person).WithMany()
            .HasForeignKey(a => a.PersonId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.MonetaryAccount).WithMany()
            .HasForeignKey(a => a.MonetaryAccountId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.CurrencyExchange).WithMany()
            .HasForeignKey(a => a.CurrencyExchangeId).OnDelete(DeleteBehavior.Restrict);

        // Exactly one owner FK populated — the DB-level guarantee that makes
        // an invalid attachment physically unrepresentable, not just app-checked.
        builder.ToTable(t => t.HasCheckConstraint("CK_Attachment_ExactlyOneOwner",
            "(CASE WHEN [AccountingDocumentId] IS NOT NULL THEN 1 ELSE 0 END + " +
            " CASE WHEN [PersonId] IS NOT NULL THEN 1 ELSE 0 END + " +
            " CASE WHEN [MonetaryAccountId] IS NOT NULL THEN 1 ELSE 0 END + " +
            " CASE WHEN [CurrencyExchangeId] IS NOT NULL THEN 1 ELSE 0 END) = 1"));

        builder.HasIndex(a => a.AccountingDocumentId);
        builder.HasIndex(a => a.PersonId);
        builder.HasIndex(a => a.MonetaryAccountId);
        builder.HasIndex(a => a.CurrencyExchangeId);
    }
}
