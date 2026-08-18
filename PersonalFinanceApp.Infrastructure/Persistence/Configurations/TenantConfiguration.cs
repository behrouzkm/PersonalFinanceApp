using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(t=>t.Id);
        builder.Property(t=>t.Name).IsRequired().HasMaxLength(200);

        // Tenant is the root of multi-tenancy - it does not itself belong to a tenant,
        // so (correctly) it is not BaseAuditableEntity and gets no query filter.
        builder.HasOne<Currency>()
            .WithMany()
            .HasForeignKey(t => t.DefaultCurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Language>()
            .WithMany()
            .HasForeignKey(t => t.DefaultLanguageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
