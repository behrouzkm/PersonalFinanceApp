using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.CashAccounts.Commands.DeleteCashAccount;

public class DeleteCashAccountCommandValidator : AbstractValidator<DeleteCashAccountCommand>
{
    public DeleteCashAccountCommandValidator()
    {

        RuleFor(p => p.CashAccountId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.CashAccount.CashAccountIdRequired);


    }
}
