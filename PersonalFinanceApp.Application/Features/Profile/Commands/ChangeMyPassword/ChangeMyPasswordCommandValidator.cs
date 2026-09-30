using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Profile.Commands.ChangeMyPassword;

public class ChangeMyPasswordCommandValidator : AbstractValidator<ChangeMyPasswordCommand>
{
    public ChangeMyPasswordCommandValidator()
    {
        // Only presence is checked here - actual password policy (length,
        // complexity) is UserManager's job via ChangePasswordAsync, same as
        // registration already relies on Identity's own policy rather than
        // duplicating it in FluentValidation.
        RuleFor(x => x.CurrentPassword).NotEmpty().WithErrorCode(ApplicationErrorCodes.Auth.PasswordRequired);
        RuleFor(x => x.NewPassword).NotEmpty().WithErrorCode(ApplicationErrorCodes.Auth.PasswordRequired);
    }
}
