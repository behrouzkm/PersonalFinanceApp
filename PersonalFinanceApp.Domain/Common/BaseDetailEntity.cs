using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Domain.Common;

public abstract class BaseDetailEntity : BaseAuditableEntity
{
    public string? Description { get; private  set; }

    protected BaseDetailEntity() { }

    protected BaseDetailEntity(Guid tenantId, Guid createdBy, string? description = null) : base(tenantId, createdBy)
    {
        SetDescription(description);
    }

    public void SetDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}
