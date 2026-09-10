using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Features.Currencies.Queries.GetCurrencyById;

namespace PersonalFinanceApp.Application.Features.Currencies.Queries.GetCurrencyByCode;

public class GetCurrencyByCodeQueryValidator : AbstractValidator<GetCurrencyByCodeQuery>
{
    public GetCurrencyByCodeQueryValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Currency.CodeRequired)
            .Length(2).WithErrorCode(ApplicationErrorCodes.Currency.InvalidCurrencyCode)
            .Matches("^[a-zA-Z]{2}$").WithErrorCode(ApplicationErrorCodes.Currency.InvalidCurrencyCode);

    }
}
