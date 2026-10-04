using System;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Profile.Commands.UpdateMyProfile;

public class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
{
    public UpdateMyProfileCommandValidator()
    {
        // Reusing the exact same codes RegisterCommandValidator already uses
        // for these two fields - not a Profile-scoped duplicate.
        RuleFor(x => x.FirstName).NotEmpty().WithErrorCode(ApplicationErrorCodes.Auth.FirstNameRequired);
        RuleFor(x => x.LastName).NotEmpty().WithErrorCode(ApplicationErrorCodes.Auth.LastNameRequired);

        RuleFor(x => x.DateOfBirth)
            .Must(d => d is null || d.Value <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithErrorCode(ApplicationErrorCodes.Profile.DateOfBirthInFuture);

        RuleFor(x => x.LanguageId)
            .NotEqual(0)
            .When(x => x.LanguageId.HasValue)
            .WithErrorCode(ApplicationErrorCodes.Auth.DefaultLanguageRequired);

    }
}
