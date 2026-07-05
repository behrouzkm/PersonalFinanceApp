using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    // Multi-tenancy support
    public Guid TenantId { get; private set; }

    protected BaseEntity() { }

    protected BaseEntity(Guid tenantId)
    {
        TenantId = tenantId;
    }

}
