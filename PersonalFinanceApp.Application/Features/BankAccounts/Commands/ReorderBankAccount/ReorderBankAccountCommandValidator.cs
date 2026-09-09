using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.BankAccounts.Commands.ReorderBankAccount;

public class ReorderBankAccountCommandValidator : AbstractValidator<ReorderBankAccountCommand>
{
    public ReorderBankAccountCommandValidator()
    {
        RuleFor(p => p.BankAccountId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.BankAccount.BankAccountIdRequired);

        RuleFor(x => x.NewDisplayOrder)
            .GreaterThan(0).WithErrorCode(ApplicationErrorCodes.BankAccount.InvalidDisplayOrder);
    }
}
