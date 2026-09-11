using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceApp.Domain.Common.Constants;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Infrastructure.Persistence.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.Code).IsRequired().HasMaxLength(FieldLengths.CurrencyCode);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(FieldLengths.Name);
        builder.Property(c => c.Symbol).IsRequired().HasMaxLength(10);
        builder.Property(c => c.DisplayOrder).IsRequired();

        builder.HasIndex(c => c.Code).IsUnique();
        builder.HasIndex(c => c.DisplayOrder).IsUnique();


        builder.HasData(
            new
            {
                Id = 1,
                Code = "USD",
                Name = "US Dollar",
                DecimalPlaces = (byte)2,
                IsActive = true,
                DisplayOrder = 1,
                Symbol = "$"
            },
            new
            {
                Id = 2,
                Code = "EUR",
                Name = "Euro",
                DecimalPlaces = (byte)2,
                IsActive = true,
                DisplayOrder = 2,
                Symbol = "€"
            },
            new
            {
                Id = 3,
                Code = "IRR",
                Name = "Iranian Rial",
                DecimalPlaces = (byte)0,
                IsActive = true,
                DisplayOrder = 3,
                Symbol = "﷼"
            },
            new
            {
                Id = 4,
                Code = "TRY",
                Name = "Turkish Lira",
                DecimalPlaces = (byte)2,
                IsActive = true,
                DisplayOrder = 4,
                Symbol = "₺"
            },
            new
            {
                Id = 5,
                Code = "AED",
                Name = "UAE Dirham",
                DecimalPlaces = (byte)2,
                IsActive = true,
                DisplayOrder = 5,
                Symbol = "د.إ"
            },
            new
            {
                Id = 6,
                Code = "GBP",
                Name = "British Pound",
                DecimalPlaces = (byte)2,
                IsActive = true,
                DisplayOrder = 6,
                Symbol = "£"
            },
            new
            {
                Id = 7,
                Code = "CAD",
                Name = "Canadian Dollar",
                DecimalPlaces = (byte)2,
                IsActive = true,
                DisplayOrder = 7,
                Symbol = "C$"
            },
            new
            {
                Id = 8,
                Code = "AUD",
                Name = "Australian Dollar",
                DecimalPlaces = (byte)2,
                IsActive = true,
                DisplayOrder = 8,
                Symbol = "A$"
            },
            new
            {
                Id = 9,
                Code = "CHF",
                Name = "Swiss Franc",
                DecimalPlaces = (byte)2,
                IsActive = true,
                DisplayOrder = 9,
                Symbol = "CHF"
            },
            new
            {
                Id = 10,
                Code = "JPY",
                Name = "Japanese Yen",
                DecimalPlaces = (byte)0,
                IsActive = true,
                DisplayOrder = 10,
                Symbol = "¥"
            },
            new
            {
                Id = 11,
                Code = "CNY",
                Name = "Chinese Yuan",
                DecimalPlaces = (byte)2,
                IsActive = true,
                DisplayOrder = 11,
                Symbol = "¥"
            },
            new
            {
                Id = 12,
                Code = "SEK",
                Name = "Swedish Krona",
                DecimalPlaces = (byte)2,
                IsActive = true,
                DisplayOrder = 12,
                Symbol = "kr"
            }
        );
    }
}
