using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.BankAccounts.Queries.GetBankAccountById;

public class GetBankAccountByIdQueryValidator : AbstractValidator<GetBankAccountByIdQuery>
{
    public GetBankAccountByIdQueryValidator()
    {
        RuleFor(p => p.BankAccountId)
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.BankAccount.BankAccountIdRequired);
    }
}
