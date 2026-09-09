using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Features.CurrencyExchanges.Common;

namespace PersonalFinanceApp.Application.Features.CurrencyExchanges.Queries.GetCurrencyExchangesList;

public class GetCurrencyExchangesListQueryValidator : AbstractValidator<GetCurrencyExchangesListQuery>
{
    public GetCurrencyExchangesListQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);

        RuleFor(x => x)
            .Must(x => !x.FromDate.HasValue || !x.ToDate.HasValue || x.FromDate < x.ToDate)
            .WithErrorCode(ApplicationErrorCodes.ExpenditureList.FromLaterThanToDate);
    }
}
