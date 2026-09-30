using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Tenants.Commands.ChangeTenantLanguage;

public class ChangeTenantLanguageCommandValidator : AbstractValidator<ChangeTenantLanguageCommand>
{
    public ChangeTenantLanguageCommandValidator()
    {
        // Same rule Register already applies to DefaultLanguageId — reused,
        // not reinvented.
        RuleFor(x => x.LanguageId)
            .NotEqual(0).WithErrorCode(ApplicationErrorCodes.Auth.DefaultLanguageRequired);
    }
}
