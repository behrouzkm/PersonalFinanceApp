using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Auth.Commands.CreateTenantUser;

public class CreateTenantUserCommandValidator : AbstractValidator<CreateTenantUserCommand>
{
    public CreateTenantUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Auth.EmailRequired)
            .EmailAddress().WithErrorCode(ApplicationErrorCodes.Auth.EmailInvalid);

        RuleFor(x => x.Password)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Auth.PasswordRequired);

        RuleFor(x => x.FirstName)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Auth.FirstNameRequired);

        RuleFor(x => x.LastName)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Auth.LastNameRequired);
    }

}
