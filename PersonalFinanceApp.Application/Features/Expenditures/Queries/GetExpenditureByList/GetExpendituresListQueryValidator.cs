using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Expenditures.Queries.GetExpenditureByList;

public class GetExpendituresListQueryValidator : AbstractValidator<GetExpendituresListQuery>
{
    public GetExpendituresListQueryValidator()
    {
        RuleFor(x=>x.PageNumber).GreaterThan(0);
        RuleFor(x=>x.PageSize).InclusiveBetween(1,200);

        RuleFor(x=>x)
            .Must(x=> !x.FromDate.HasValue || !x.ToDate.HasValue || x.FromDate<x.ToDate)
            .WithErrorCode(ApplicationErrorCodes.ExpenditureList.FromLaterThanToDate);
    }

}
