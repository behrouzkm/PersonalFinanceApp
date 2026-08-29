using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Infrastructure.Persistence.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.PersonType).HasConversion<int>();
        builder.Property(p => p.DisplayName).IsRequired().HasMaxLength(200);

        builder.Property(p => p.InitialBalance).HasPrecision(18, 2);
        builder.Property(p => p.CurrentBalance).HasPrecision(18, 2);
        builder.Property(p => p.CreditLimit).HasPrecision(18, 2);

        builder.Property(p => p.Email).HasMaxLength(320);
        builder.Property(p => p.MobileNumber).HasMaxLength(20);
        builder.Property(p => p.TelNumber).HasMaxLength(20);


        builder.HasOne(p => p.LedgerAccount)
            .WithMany()
            .HasForeignKey(p => p.LedgerAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Currency)
            .WithMany()
            .HasForeignKey(p => p.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne(p => p.OpeningAccountingDocument)
            .WithMany()
            .HasForeignKey(p => p.OpeningAccountingDocumentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.LedgerAccountId).IsUnique();
    }
}
