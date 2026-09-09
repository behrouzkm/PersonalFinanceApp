using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.BankAccounts.Commands.DeleteBankAccount;

public class DeleteBankAccountCommandValidator : AbstractValidator<DeleteBankAccountCommand>
{
    public DeleteBankAccountCommandValidator()
    {

        RuleFor(p => p.BankAccountId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.BankAccount.BankAccountIdRequired);


    }
}
