using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{

    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Auth.EmailRequired)
            .EmailAddress().WithErrorCode(ApplicationErrorCodes.Auth.EmailInvalid);

        // this is for password requirement check
        // Password strength itself is enforced by ASP.NET Core Identity's own
        // configured passwordOptions
        RuleFor(x => x.Password)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Auth.PasswordRequired);

        RuleFor(x => x.TenantName)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Auth.TenantNameRequired);

        RuleFor(x => x.FirstName)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Auth.FirstNameRequired);

        RuleFor(x => x.LastName)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Auth.LastNameRequired);

        RuleFor(x => x.DefaultLanguageId)
            .NotEqual((byte)0).WithErrorCode(ApplicationErrorCodes.Auth.DefaultLanguageRequired);

        RuleFor(x => x.DefaultCurrencyId)
            .NotEqual((byte)0).WithErrorCode(ApplicationErrorCodes.Auth.DefaultCurrencyRequired);
    }

}
