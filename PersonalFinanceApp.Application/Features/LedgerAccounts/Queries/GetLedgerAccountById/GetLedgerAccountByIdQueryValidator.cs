using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Queries.GetLedgerAccountById;

public class GetLedgerAccountByIdQueryValidator : AbstractValidator<GetLedgerAccountByIdQuery>
{
    public GetLedgerAccountByIdQueryValidator()
    {
        RuleFor(p => p.LedgerAccountId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.LedgerAccount.LedgerAccountIdRequired);
    }
}
