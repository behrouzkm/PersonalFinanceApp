using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Queries.GetLedgerAccountsList;

public class GetLedgerAccountsListQueryValidator : AbstractValidator<GetLedgerAccountsListQuery>
{
    public GetLedgerAccountsListQueryValidator()
    {

        RuleFor(x => x.AccountTypeId)
            .GreaterThan(0)
            .When(x => x.AccountTypeId.HasValue)
            .WithErrorCode(ApplicationErrorCodes.LedgerAccount.InvalidAccountTypeId);

        RuleFor(x => x.ParentId)
            .NotEqual(Guid.Empty)
            .When(x => x.ParentId.HasValue)
            .WithErrorCode(ApplicationErrorCodes.LedgerAccount.InvalidParentLedgerId);

        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 20);

    }

}
