using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.CreateLedgerAccount;

public class CreateLedgerAccountCommandValidator : AbstractValidator<CreateLedgerAccountCommand>
{
    public CreateLedgerAccountCommandValidator()
    {
        RuleFor(p => p.AccountTypeId)
            .NotEmpty()
            .NotEqual(0)
            .WithErrorCode(ApplicationErrorCodes.LedgerAccount.InvalidAccountTypeId);

        RuleFor(p => p.Name)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.LedgerAccount.NameRequired);

        RuleFor(p => p.ParentId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.LedgerAccount.InvalidParentLedgerId);

    }
}
