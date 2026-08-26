using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Currencies.Commands.CreateCurrency;

public class CreateCurrencyCommandValidator : AbstractValidator<CreateCurrencyCommand>
{
    public CreateCurrencyCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Currency.CodeRequired)
            .Length(3).WithErrorCode(ApplicationErrorCodes.Currency.InvalidCurrencyCode)
            .Matches("^[a-zA-Z]{3}$").WithErrorCode(ApplicationErrorCodes.Currency.InvalidCurrencyCode);


        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.Currency.NameRequired);

        RuleFor(x => x.DecimalPlaces)
            .Must(x => x <= 3).WithErrorCode(ApplicationErrorCodes.Currency.InvalidDecimalPlaces);


        RuleFor(x => x.Symbol)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.Currency.SymbolRequired);
    }
}
