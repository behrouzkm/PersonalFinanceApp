using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Common.Constants;

/// <summary>
/// Role names as they must appear in ASP.NET Core Identity (AspNetRoles.Name).
/// Two scopes: Platform (cross-tenant, manages global lookup data and Platform
/// accounts) and Tenant (scoped to a single TenantId). Fine-grained differences
/// within a scope (e.g. PlatformSupport can't delete another Platform account)
/// are enforced in the command handlers, not by hiding pages from one role —
/// hiding a menu item is a UX nicety only, never the security boundary.
/// </summary>
public static class Roles
{
    // Platform-level roles: not scoped to any tenant. Manage global lookup data
    // (Language, Currency, AccountType) and Platform-level accounts themselves.
    public const string PlatformAdministrators = "PlatformAdministrators"; // Full platform admin. The only role that can delete or deactivate another Platform account (Administrator or Support).
    public const string PlatformSupport = "PlatformSupport"; // Same admin-area access and cross-tenant lookup management as PlatformAdministrators, but cannot delete or deactivate other Platform accounts.

    // Tenant-level roles: scoped to a single TenantId via ApplicationUser.TenantId.
    public const string TenantAdministrators = "TenantAdministrators"; // Owns the tenant (exactly one per tenant). Invites/manages Users and Roles within it. The only tenant role that can view deleted records and restore them.
    public const string TenantPowerUsers = "TenantPowerUsers"; // Full CRUD on tenant data (create/edit/delete), but cannot view deleted-records lists or restore — that surface is Administrator-only.
    public const string TenantMembers = "TenantMembers"; // Regular tenant member. No admin surface, no deleted-records access.
}
