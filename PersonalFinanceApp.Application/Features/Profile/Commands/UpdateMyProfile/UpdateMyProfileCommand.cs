using System;
using MediatR;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.Profile.Commands.UpdateMyProfile;

public class UpdateMyProfileCommand : IRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }

    // Null means "use the tenant's default language" rather than a personal
    // override - same null-means-fallback convention CreateUserForExistingTenantAsync
    // already uses for the same column.
    public int? LanguageId { get; set; }
}
