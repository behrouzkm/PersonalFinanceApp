using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Infrastructure.Persistence.Configurations;

public class MoneyTransferConfiguration : IEntityTypeConfiguration<MoneyTransfer>
{
    public void Configure(EntityTypeBuilder<MoneyTransfer> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Amount).HasPrecision(18, 2);

        builder.HasOne(t => t.FromMonetaryAccount)
            .WithMany()
            .HasForeignKey(t => t.FromMonetaryAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.ToMonetaryAccount)
            .WithMany()
            .HasForeignKey(t => t.ToMonetaryAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new { t.TenantId, t.TransferDate });
    }
}
