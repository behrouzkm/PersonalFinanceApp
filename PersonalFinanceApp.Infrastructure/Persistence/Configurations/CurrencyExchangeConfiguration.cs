using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceApp.Domain.Common.Constants;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Infrastructure.Persistence.Configurations;

public class MoneyTransferConfiguration : IEntityTypeConfiguration<CurrencyExchange>
{
    public void Configure(EntityTypeBuilder<CurrencyExchange> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(e=>e.RowVersion).IsRowVersion();

        builder.Property(t => t.ExchangeRate).HasPrecision(DecimalPrecision.Precision, DecimalPrecision.ExchangeRateScale);

        builder.HasOne(t => t.FromDocument)
            .WithMany()
            .HasForeignKey(t => t.FromDocumentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.ToDocument)
            .WithMany()
            .HasForeignKey(t => t.ToDocumentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.IsDeleted });
    }
}
