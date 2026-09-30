using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Queries.GetMoneyTransfersDeletedList;

public sealed class GetMoneyTransfersDeletedListQueryValidator
    : AbstractValidator<GetMoneyTransfersDeletedListQuery>
{
    public GetMoneyTransfersDeletedListQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 200);

        
    }
}
