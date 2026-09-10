using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceApp.Domain.Common.Constants;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Infrastructure.Persistence.Configurations;

public class AccountTypeTranslationConfiguration : IEntityTypeConfiguration<AccountTypeTranslation>
{
    public void Configure(EntityTypeBuilder<AccountTypeTranslation> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();

        builder.Property(a => a.Translation).IsRequired().HasMaxLength(FieldLengths.Name);
        builder.Property(a => a.Description).HasMaxLength(FieldLengths.Description);

        builder.HasOne(a => a.AccountType)
            .WithMany()
            .HasForeignKey(a => a.AccountTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Language)
            .WithMany()
            .HasForeignKey(t => t.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.AccountTypeId, a.LanguageId }).IsUnique();

    }

}
