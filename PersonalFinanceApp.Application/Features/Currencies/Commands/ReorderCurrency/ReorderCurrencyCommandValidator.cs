using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Currencies.Commands.ReorderCurrency;

public class ReorderCurrencyCommandValidator : AbstractValidator<ReorderCurrencyCommand>
{
    public ReorderCurrencyCommandValidator()
    {
        RuleFor(x=>x.Id)
            .GreaterThan(0).WithErrorCode(ApplicationErrorCodes.Currency.IdRequired);

        RuleFor(x => x.NewDisplayOrder)
            .GreaterThan(0).WithErrorCode(ApplicationErrorCodes.Currency.InvalidDisplayOrder);
    }
}
