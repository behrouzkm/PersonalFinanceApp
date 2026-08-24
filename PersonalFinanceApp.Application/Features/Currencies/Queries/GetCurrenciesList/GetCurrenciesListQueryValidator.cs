using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Currencies.Queries.GetCurrenciesList;

public class GetCurrenciesListQueryValidator : AbstractValidator<GetCurrenciesListQuery>
{
    public GetCurrenciesListQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 20);

    }

}
