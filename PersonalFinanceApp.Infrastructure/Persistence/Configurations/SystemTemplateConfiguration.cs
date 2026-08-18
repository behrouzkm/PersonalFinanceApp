using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Infrastructure.Persistence.Configurations;

public class SystemTemplateConfiguration : IEntityTypeConfiguration<SystemTemplate>
{
    public void Configure(EntityTypeBuilder<SystemTemplate> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder.Property(t => t.TemplateKey).IsRequired().HasMaxLength(200);
        builder.Property(t => t.JsonData).IsRequired(); // no max lenght - can be large

        builder.HasOne(t => t.Language)
            .WithMany()
            .HasForeignKey(t => t.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new { t.TemplateKey, t.LanguageId }).IsUnique();
    }
}
