using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Queries.GetMoneyTransfersList;

public sealed class GetMoneyTransfersListQueryValidator
    : AbstractValidator<GetMoneyTransfersListQuery>
{
    public GetMoneyTransfersListQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 200);

        RuleFor(x => x.SearchText)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.SearchText));

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithErrorCode(ApplicationErrorCodes.MoneyTransfer.FromLaterThanToDate);

        RuleFor(x => x.FromAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.FromAmount.HasValue);

        RuleFor(x => x.ToAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.ToAmount.HasValue);

        RuleFor(x => x.FromAmount)
            .LessThanOrEqualTo(x => x.ToAmount)
            .When(x => x.FromAmount.HasValue && x.ToAmount.HasValue)
            .WithErrorCode(ApplicationErrorCodes.MoneyTransfer.FromAmountGreaterThanToAmount);
    }
}
