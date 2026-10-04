using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<IdentityRegistrationResult> RegisterAsync(
        string email,
        string password,
        string tenantName,
        string firstName,
        string lastName,
        int defaultLanguageId,
        int defaultCurrencyId,
        CancellationToken cancellationToken
    );

    // no tenant is created here - tenantId must already exist
    Task<IdentityRegistrationResult> CreateUserForExistingTenantAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        Guid tenantId,
        int? languageId,
        CancellationToken cancellationToken
    );


    Task<IdentityLoginResult> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken
    );


    Task<IdentityUserProfileResult> GetProfileAsync(Guid userId, CancellationToken cancellationToken);

    Task<IdentityOperationResult> UpdateProfileAsync(
        Guid userId, string firstName, string lastName, DateOnly? dateOfBirth, Gender? gender, int? languageId,
        CancellationToken cancellationToken);

    // storageKey null clears the photo (used when removing it without replacing).
    Task<IdentityOperationResult> SetProfilePhotoAsync(
        Guid userId, string? storageKey, CancellationToken cancellationToken);

    Task<IdentityOperationResult> ChangePasswordAsync(
        Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken);
}

public class IdentityRegistrationResult
{
    public bool Succeeded { get; init; }
    public Guid? UserId { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();
}

public class IdentityLoginResult
{
    public bool Succeeded { get; init; }
    public string? Token { get; init; }
    public DateTime ExpiresAtUtc { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();
}

public class IdentityOperationResult
{
    public bool Succeeded { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

    public static IdentityOperationResult Success() => new() { Succeeded = true };
    public static IdentityOperationResult Failure(IEnumerable<string> errors) =>
        new() { Succeeded = false, Errors = errors.ToList() };
}

public class IdentityUserProfileResult
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? ProfilePhotoStorageKey { get; init; }
    public DateOnly? DateOfBirth { get; init; }
    public Gender? Gender { get; init; }
    public int? LanguageId { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? LastLoginAtUtc { get; init; }
    public DateTime? PasswordChangedAtUtc { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
}
