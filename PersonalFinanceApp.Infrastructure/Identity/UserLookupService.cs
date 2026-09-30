using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Infrastructure.Persistence;

namespace PersonalFinanceApp.Infrastructure.Identity;

public class UserLookupService : IUserLookupService
{
    private readonly ApplicationDbContext _context;

    public UserLookupService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<UserSummaryDto>> GetTenantUsersAsync(
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.TenantId == tenantId)
            .Select(u => new UserSummaryDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName
            })
            .ToListAsync(cancellationToken);
    }

}
