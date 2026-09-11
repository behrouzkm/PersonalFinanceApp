using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceApp.Domain.Common.Constants;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Infrastructure.Persistence.Seed;

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


        var rows = SeedJsonLoader.Load<AccountTypeTranslationSeedRow>("account-type-translations.json");
        var typeIds = new Dictionary<string, int> { ["Expense"] = 1, ["Income"] = 2, ["Person"] = 3, ["Bank"] = 4, ["Cash"] = 5, ["CurrencyExchangeClearing"] = 6, ["OpeningBalanceEquity"] = 7 };
        var langIds = new Dictionary<string, int> { ["en"] = 1, ["fa"] = 2, ["tr"] = 3, ["fr"] = 4, ["de"] = 5, ["ar"] = 6, ["es"] = 7, ["it"] = 8, ["ru"] = 9, ["zh"] = 10, ["ja"] = 11, ["sv"] = 12 };

        builder.HasData(rows.Select((r, i) => new
        {
            Id = i + 1,
            AccountTypeId = typeIds[r.AccountTypeCode],
            LanguageId = langIds[r.LanguageCode],
            Translation = r.Translation
        }));

    }

}
