using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.CashAccounts.Commands.RestoreCashAccount;

public class RestoreCashAccountCommandValidator : AbstractValidator<RestoreCashAccountCommand>
{
    public RestoreCashAccountCommandValidator()
    {
        RuleFor(x=> x.CashAccountId)
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.CashAccount.CashAccountIdRequired);
    }
}
