using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceApp.Domain.Common.Constants;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Infrastructure.Persistence.Configurations;

public class LedgerAccountConfiguration : IEntityTypeConfiguration<LedgerAccount>
{
    public void Configure(EntityTypeBuilder<LedgerAccount> builder)
    {
        builder.HasKey(a => a.Id);

        //builder.Property(a => a.RowVersion).IsRowVersion();

        builder.Property(a => a.Name).IsRequired().HasMaxLength(FieldLengths.Name);

        //Self-referencing
        builder.HasOne(a => a.Parent)
            .WithMany(a => a.Children)
            .HasForeignKey(a => a.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(a => a.Children)
            .HasField("_children")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne(a => a.AccountType)
            .WithMany()
            .HasForeignKey(a => a.AccountTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.TenantId, a.ParentId });
        builder.HasIndex(a => new { a.TenantId, a.ParentId, a.DisplayOrder }).IsUnique().HasFilter("[IsDeleted] = 0");
    }
}
