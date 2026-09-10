using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.AccountTypeTranslations.Queries.GetAccountTypeTranslationsList;

public class GetAccountTypeTranslationsListQueryValidator : AbstractValidator<GetAccountTypeTranslationsListQuery>
{
    public GetAccountTypeTranslationsListQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 20);

    }

}
