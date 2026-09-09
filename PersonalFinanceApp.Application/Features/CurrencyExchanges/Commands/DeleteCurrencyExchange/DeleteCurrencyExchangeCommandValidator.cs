using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.CurrencyExchanges.Commands.DeleteCurrencyExchange;

public class DeleteCurrencyExchangeCommandValidator : AbstractValidator<DeleteCurrencyExchangeCommand>
{
    public DeleteCurrencyExchangeCommandValidator()
    {
        RuleFor(x => x.CurrencyExchangeId)
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.CurrencyExchange.DocumentIdRequired);

        RuleFor(x => x.RowVersion)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.CurrencyExchange.RowVersionRequired);

    }

}
