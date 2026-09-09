using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.CashAccounts.Commands.ReorderCashAccount;

public class ReorderCashAccountCommandValidator : AbstractValidator<ReorderCashAccountCommand>
{
    public ReorderCashAccountCommandValidator()
    {
        RuleFor(p => p.CashAccountId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.CashAccount.CashAccountIdRequired);

        RuleFor(x => x.NewDisplayOrder)
            .GreaterThan(0).WithErrorCode(ApplicationErrorCodes.CashAccount.InvalidDisplayOrder);
    }
}
