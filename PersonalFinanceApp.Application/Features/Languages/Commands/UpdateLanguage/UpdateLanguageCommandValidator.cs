using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Languages.Commands.UpdateLanguage;

public class UpdateLanguageCommandValidator : AbstractValidator<UpdateLanguageCommand>
{
    public UpdateLanguageCommandValidator()
    {


        RuleFor(x => x.Code)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Currency.CodeRequired)
            .Length(2).WithErrorCode(ApplicationErrorCodes.Currency.InvalidCurrencyCode)
            .Matches("^[a-zA-Z]{3}$").WithErrorCode(ApplicationErrorCodes.Currency.InvalidCurrencyCode);


        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.Language.NameRequired);

    }
}
