using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Features.LedgerAccounts.Queries.GetLedgerAccountsOptions;

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.CreateLedgerAccount;

public class GetLedgerAccountsOptionsQueryValidator : AbstractValidator<GetLedgerAccountsOptionsQuery>
{
    public GetLedgerAccountsOptionsQueryValidator()
    {
        RuleFor(p => p.AccountTypeId)
            .NotEmpty()
            .NotEqual(0)
            .WithErrorCode(ApplicationErrorCodes.LedgerAccount.InvalidAccountTypeId);

        RuleFor(p => p.ParentId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.LedgerAccount.InvalidParentLedgerId);

    }
}
