using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceApp.Domain.Common.Constants;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Infrastructure.Persistence.Configurations;

public class DocumentTypeTranslationConfiguration : IEntityTypeConfiguration<DocumentTypeTranslation>
{
    public void Configure(EntityTypeBuilder<DocumentTypeTranslation> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedOnAdd();

        builder.Property(d => d.DocumentType).HasConversion<int>();

        builder.Property(d => d.Name).IsRequired().HasMaxLength(FieldLengths.Name);
        builder.Property(d => d.Description).HasMaxLength(FieldLengths.Description);

        builder.HasOne(d => d.Language)
            .WithMany()
            .HasForeignKey(d => d.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => new { d.DocumentType, d.LanguageId }).IsUnique();
    }
}
