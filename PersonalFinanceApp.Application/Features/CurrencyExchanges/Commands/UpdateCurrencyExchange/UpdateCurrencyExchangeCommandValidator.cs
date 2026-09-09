using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.CurrencyExchanges.Commands.UpdateCurrencyExchange;

public class UpdateCurrencyExchangeCommandValidator : AbstractValidator<UpdateCurrencyExchangeCommand>
{

    public UpdateCurrencyExchangeCommandValidator()
    {
        RuleFor(x => x.CurrencyExchangeId)
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.CurrencyExchange.DocumentIdRequired);

        RuleFor(x => x.ExchangeDate)
           .NotEmpty().WithErrorCode(ApplicationErrorCodes.CurrencyExchange.ExchangeDateRequired)
           .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
           .WithErrorCode(ApplicationErrorCodes.CurrencyExchange.ExchangeDateInFuture);

        RuleFor(x => x.FromLedgerAccountId).NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.CurrencyExchange.FromLedgerAccountRequired);

        RuleFor(x => x.ToLedgerAccountId).NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.CurrencyExchange.ToLedgerAccountRequired);

        RuleFor(x => x)
            .Must(x => x.FromLedgerAccountId != x.ToLedgerAccountId)
            .WithErrorCode(ApplicationErrorCodes.CurrencyExchange.SourceAndDestinationMustDiffer);

        RuleFor(x => x.FromAmount).GreaterThan(0)
            .WithErrorCode(ApplicationErrorCodes.CurrencyExchange.FromAmountMustBePositive);

        RuleFor(x => x.ToAmount).GreaterThan(0)
            .WithErrorCode(ApplicationErrorCodes.CurrencyExchange.ToAmountMustBePositive);

        RuleFor(x => x.ExchangeRate).GreaterThan(0)
            .WithErrorCode(ApplicationErrorCodes.CurrencyExchange.ExchangeRateMustBePositive);

        // Tolerance is a judgment call — 1% here as a placeholder against real-world
        // rounding on both legs. Tune against how your UI actually computes ToAmount.
        RuleFor(x => x)
            .Must(x => Math.Abs(x.ToAmount - x.FromAmount * x.ExchangeRate) <= x.ToAmount * 0.01m)
            .WithErrorCode(ApplicationErrorCodes.CurrencyExchange.AmountRateMismatch);

        RuleFor(x => x.RowVersion)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.CurrencyExchange.RowVersionRequired);


    }
}
