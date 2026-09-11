

namespace PersonalFinanceApp.Infrastructure.Identity;

public class DevelopmentSeedOptions
{
    public bool Enabled { get; set; }
    public string TenantName { get; set; } = "Demo Tenant";
    public string AdminEmail { get; set; } = "admin@localhost";
    public string AdminPassword { get; set; } = default!; // from user-secrets only, never appsettings.json
}





