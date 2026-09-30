using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public int? LanguageId { get; set; }

    public string? ProfilePhotoStorageKey { get; set; }

    public DateOnly? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Updated on every successful LoginAsync call.
    public DateTime? LastLoginAtUtc { get; set; }

    // Updated only by ChangePasswordAsync - null until the user has changed
    // their password at least once since registration.
    public DateTime? PasswordChangedAtUtc { get; set; }
}
