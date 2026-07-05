using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow; // Set to UTC time for consistency across time zones
    public Guid CreatedBy { get; private set; }

    public DateTime? LastModifiedAt { get; private set; }
    public Guid? LastModifiedBy { get; private set; }

    // Soft delete properties
    public bool IsDeleted { get; private set; }


    protected BaseAuditableEntity() { }

    protected BaseAuditableEntity(Guid tenantId, Guid createdBy) : base(tenantId)
    {
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateAudit(Guid modifiedBy)
    {
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    public void SoftDelete(Guid deletedBy)
    {
        IsDeleted = true;
        UpdateAudit(deletedBy);
    }

    public void Restore(Guid restoredBy)
    {
        IsDeleted = false;
        UpdateAudit(restoredBy);
    }


}
