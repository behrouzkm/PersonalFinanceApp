using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Domain.Common;

public abstract class BaseDetailEntity : BaseAuditableEntity
{


    protected BaseDetailEntity() { }

    protected BaseDetailEntity(Guid tenantId, Guid createdBy, string? description = null)
                                : base(tenantId, createdBy, description)
    {

    }


}
