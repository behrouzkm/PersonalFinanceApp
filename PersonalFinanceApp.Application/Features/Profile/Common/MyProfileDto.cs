using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Features.Currencies.Common;
using PersonalFinanceApp.Application.Features.Languages.Common;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.Profile.Common;


public class MyProfileDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfilePhotoStorageKey { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastLoginAtUtc { get; set; }
    public DateTime? PasswordChangedAtUtc { get; set; }

    // Reused DTOs - the exact same shapes the Currencies/Languages features
    // already return elsewhere, not a parallel set of profile-specific types.
    public LanguageOptionDto TenantLanguage { get; set; } = null!;
    public CurrencyOptionDto TenantCurrency { get; set; } = null!;

    // Server-computed, not just a UI hint - the Blazor page uses this to
    // decide whether to render the language/currency fields as editable, but
    // the real enforcement is [Authorize(Roles = Roles.TenantAdministrators)]
    // on the write endpoints themselves (see TenantController).
    public bool CanEditTenantSettings { get; set; }
}
