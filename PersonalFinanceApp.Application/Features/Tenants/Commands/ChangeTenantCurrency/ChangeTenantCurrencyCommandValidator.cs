using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Tenants.Commands.ChangeTenantCurrency;

public class ChangeTenantCurrencyCommandValidator : AbstractValidator<ChangeTenantCurrencyCommand>
{
    public ChangeTenantCurrencyCommandValidator()
    {
        RuleFor(x => x.CurrencyId)
            .NotEqual(0).WithErrorCode(ApplicationErrorCodes.Auth.DefaultCurrencyRequired);
    }
}
