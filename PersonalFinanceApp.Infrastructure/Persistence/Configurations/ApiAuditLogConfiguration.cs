using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Infrastructure.Persistence.Configurations;

public class ApiAuditLogConfiguration : IEntityTypeConfiguration<ApiAuditLog>
{
    public void Configure(EntityTypeBuilder<ApiAuditLog> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.HttpMethod).IsRequired().HasMaxLength(10);
        builder.Property(a => a.RequestPath).IsRequired().HasMaxLength(2000);
        builder.Property(a => a.ControllerName).HasMaxLength(200);
        builder.Property(a => a.ActionName).HasMaxLength(200);
        builder.Property(a => a.CorrelationId).IsRequired().HasMaxLength(100);
        builder.Property(a => a.IpAddress).HasMaxLength(50);
        builder.Property(a => a.UserAgent).HasMaxLength(500);

        // No FK constraints on UserId/TenantId, deliberately - an audit row must stay
        // queryable even if the user or tenant it references is later deleted. These
        // are informational references, not enforced relationships.
        builder.HasIndex(a => a.TenantId);
        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.Timestamp);
        builder.HasIndex(a => a.CorrelationId);
    }
}
