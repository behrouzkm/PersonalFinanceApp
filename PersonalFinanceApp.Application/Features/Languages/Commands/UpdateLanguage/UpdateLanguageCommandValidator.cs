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
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Language.CodeRequired)
            .Length(2).WithErrorCode(ApplicationErrorCodes.Language.InvalidLanguageCode)
            .Matches("^[a-zA-Z]{2}$").WithErrorCode(ApplicationErrorCodes.Language.InvalidLanguageCode);


        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.Language.NameRequired);

    }
}
