using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.AccountTypeTranslations.Commands.CreateAccountTypeTranslation;

public class CreateAccountTypeTranslationCommandValidator : AbstractValidator<CreateAccountTypeTranslationCommand>
{
    public CreateAccountTypeTranslationCommandValidator()
    {
        RuleFor(x => x.AccountTypeId)
            .GreaterThan(0).WithErrorCode(ApplicationErrorCodes.AccountTypeTranslation.AccountTypeTranslationIdRequired);

        RuleFor(x => x.LanguageId)
             .GreaterThan(0).WithErrorCode(ApplicationErrorCodes.AccountTypeTranslation.LanguageIdRequired);

        RuleFor(x => x.Translation)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.AccountTypeTranslation.TranslationRequired);

    }
}
