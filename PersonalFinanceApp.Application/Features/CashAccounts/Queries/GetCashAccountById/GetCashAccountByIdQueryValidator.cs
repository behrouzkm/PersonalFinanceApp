using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.CashAccounts.Queries.GetCashAccountById;

public class GetCashAccountByIdQueryValidator : AbstractValidator<GetCashAccountByIdQuery>
{
    public GetCashAccountByIdQueryValidator()
    {
        RuleFor(p => p.CashAccountId)
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.CashAccount.CashAccountIdRequired);
    }
}
