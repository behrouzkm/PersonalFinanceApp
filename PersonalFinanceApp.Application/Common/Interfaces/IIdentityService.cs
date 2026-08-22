using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<IdentityRegistrationResult> RegisterAsync(
        string email,
        string password,
        string tenantName,
        string firstName,
        string lastName,
        byte defaultLanguageId,
        byte defaultCurrencyId,
        CancellationToken cancellationToken
    );

    // no tenant is created here - tenantId must already exist
    Task<IdentityRegistrationResult> CreateUserForExistingTenantAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        Guid tenantId,
        CancellationToken cancellationToken
    );


    Task<IdentityLoginResult> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken
    );
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
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();
}
