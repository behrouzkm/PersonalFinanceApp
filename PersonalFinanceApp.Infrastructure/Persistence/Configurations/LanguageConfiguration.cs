using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceApp.Domain.Common.Constants;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Infrastructure.Persistence.Configurations;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedOnAdd();

        builder.Property(l => l.Code).IsRequired().HasMaxLength(FieldLengths.LanguageCode);
        builder.Property(l => l.Name).IsRequired().HasMaxLength(FieldLengths.Name);
        builder.Property(l => l.DisplayOrder).IsRequired();


        builder.HasIndex(l => l.Code).IsUnique();
        builder.HasIndex(l => l.DisplayOrder).IsUnique();


        builder.HasData(
            new
            {
                Id = 1,
                Code = "en",
                Name = "English",
                IsActive = true,
                DisplayOrder = 1,
                IsRightToLeft = false
            },
            new
            {
                Id = 2,
                Code = "fa",
                Name = "فارسی",
                IsActive = true,
                DisplayOrder = 2,
                IsRightToLeft = true
            },
            new
            {
                Id = 3,
                Code = "tr",
                Name = "Türkçe",
                IsActive = true,
                DisplayOrder = 3,
                IsRightToLeft = false
            },
            new
            {
                Id = 4,
                Code = "fr",
                Name = "Français",
                IsActive = true,
                DisplayOrder = 4,
                IsRightToLeft = false
            },
            new
            {
                Id = 5,
                Code = "de",
                Name = "Deutsch",
                IsActive = true,
                DisplayOrder = 5,
                IsRightToLeft = false
            },
            new
            {
                Id = 6,
                Code = "ar",
                Name = "العربية",
                IsActive = true,
                DisplayOrder = 6,
                IsRightToLeft = true
            },
            new
            {
                Id = 7,
                Code = "es",
                Name = "Español",
                IsActive = true,
                DisplayOrder = 7,
                IsRightToLeft = false
            },
            new
            {
                Id = 8,
                Code = "it",
                Name = "Italiano",
                IsActive = true,
                DisplayOrder = 8,
                IsRightToLeft = false
            },
            new
            {
                Id = 9,
                Code = "ru",
                Name = "Русский",
                IsActive = true,
                DisplayOrder = 9,
                IsRightToLeft = false
            },
            new
            {
                Id = 10,
                Code = "zh",
                Name = "中文",
                IsActive = true,
                DisplayOrder = 10,
                IsRightToLeft = false
            },
            new
            {
                Id = 11,
                Code = "ja",
                Name = "日本語",
                IsActive = true,
                DisplayOrder = 11,
                IsRightToLeft = false
            },
            new
            {
                Id = 12,
                Code = "sv",
                Name = "Svenska",
                IsActive = true,
                DisplayOrder = 12,
                IsRightToLeft = false
            }
        );
    }
}
