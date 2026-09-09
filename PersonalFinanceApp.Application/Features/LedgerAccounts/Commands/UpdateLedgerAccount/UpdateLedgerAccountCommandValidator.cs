using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.UpdateLedgerAccount;

public class UpdateLedgerAccountCommandValidator : AbstractValidator<UpdateLedgerAccountCommand>
{
    public UpdateLedgerAccountCommandValidator()
    {

        RuleFor(p => p.LedgerAccountId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.LedgerAccount.LedgerAccountIdRequired);

        RuleFor(p => p.Name)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.LedgerAccount.NameRequired);
    }
}
