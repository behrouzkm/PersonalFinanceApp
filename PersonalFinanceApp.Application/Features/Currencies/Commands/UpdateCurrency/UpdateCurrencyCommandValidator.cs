using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Currencies.Commands.UpdateCurrency;

public class UpdateCurrencyCommandValidator : AbstractValidator<UpdateCurrencyCommand>
{
    public UpdateCurrencyCommandValidator()
    {


        RuleFor(x => x.Code)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Currency.CodeRequired)
            .Length(3).WithErrorCode(ApplicationErrorCodes.Currency.InvalidCurrencyCode);


        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.Currency.NameRequired);

        RuleFor(x => x.DecimalPlaces)
            .InclusiveBetween(0, 2).WithErrorCode(ApplicationErrorCodes.Currency.InvalidDecimalPlaces)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Currency.DecimalRequired);


        RuleFor(x => x.Symbol)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.Currency.SymbolRequired);
    }
}
