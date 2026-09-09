using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.ReorderLedgerAccount;

public class ReorderLedgerAccountCommandValidator : AbstractValidator<ReorderLedgerAccountCommand>
{
    public ReorderLedgerAccountCommandValidator()
    {
        RuleFor(p => p.LedgerAccountId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.LedgerAccount.LedgerAccountIdRequired);

        RuleFor(p => p.ParentId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.LedgerAccount.InvalidParentLedgerId);

        RuleFor(x => x.NewDisplayOrder)
            .GreaterThan(0).WithErrorCode(ApplicationErrorCodes.LedgerAccount.InvalidDisplayOrder);
    }
}
