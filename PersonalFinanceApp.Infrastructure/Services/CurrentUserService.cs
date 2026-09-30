using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PersonalFinanceApp.Application.Common.Interfaces;

namespace PersonalFinanceApp.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // Claim type names ("tenant_id", NameIdentifier) are placeholders - adjust to
    // match whatever auth scheme/JWT claims you actually issue once auth is wired up.
    public Guid TenantId => GetGuidClaim("tenant_id");

    public Guid UserId => GetGuidClaim(ClaimTypes.NameIdentifier);

    // TokenService already embeds roles as ClaimTypes.Role claims (see
    // GenerateToken), so this is a direct, zero-extra-plumbing read.
    public bool IsInRole(string role) =>
        _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;


    private Guid GetGuidClaim(string claimType)
    {
        var value = _httpContextAccessor.HttpContext?.User?.FindFirstValue(claimType);

        // returning Guid.Empty when unauthenticated/unclaimed
        return Guid.TryParse(value, out var guid)
            ? guid
            : throw new UnauthorizedAccessException();
    }

}
