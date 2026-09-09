using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.CurrencyExchanges.Commands.RestoreCurrencyExchange;

public class RestoreCurrencyExchangeCommandValidator : AbstractValidator<RestoreCurrencyExchangeCommand>
{
    public RestoreCurrencyExchangeCommandValidator()
    {
        RuleFor(x => x.CurrencyExchangeId)
           .NotEqual(Guid.Empty)
           .WithErrorCode(ApplicationErrorCodes.CurrencyExchange.DocumentIdRequired);
    }
}
