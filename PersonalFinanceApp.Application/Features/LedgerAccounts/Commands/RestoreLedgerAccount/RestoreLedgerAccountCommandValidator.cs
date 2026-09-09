using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.RestoreLedgerAccount;

public class RestoreLedgerAccountCommandValidator : AbstractValidator<RestoreLedgerAccountCommand>
{
    public RestoreLedgerAccountCommandValidator()
    {
        RuleFor(x => x.LedgerAccountId)
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.LedgerAccount.LedgerAccountIdRequired);
    }
}
