using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.BankAccounts.Commands.RestoreBankAccount;

public class RestoreBankAccountCommandValidator : AbstractValidator<RestoreBankAccountCommand>
{
    public RestoreBankAccountCommandValidator()
    {
        RuleFor(x=> x.BankAccountId)
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.BankAccount.BankAccountIdRequired);
    }
}
