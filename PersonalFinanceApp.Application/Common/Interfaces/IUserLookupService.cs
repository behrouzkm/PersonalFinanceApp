using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Common.Models;

namespace PersonalFinanceApp.Application.Common.Interfaces;

public interface IUserLookupService
{
    Task<IReadOnlyList<UserSummaryDto>> GetTenantUsersAsync(Guid tenantId, CancellationToken cancellationToken);
}
