using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Auth.EmailRequired)
            .EmailAddress().WithErrorCode(ApplicationErrorCodes.Auth.EmailInvalid);

        RuleFor(x => x.Password)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Auth.PasswordRequired);
    }
}
