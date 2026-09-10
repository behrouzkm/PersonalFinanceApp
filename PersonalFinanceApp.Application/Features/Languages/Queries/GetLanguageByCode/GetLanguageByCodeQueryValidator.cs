using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Features.Languages.Queries.GetLanguageById;

namespace PersonalFinanceApp.Application.Features.Languages.Queries.GetLanguageByCode;

public class GetLanguageByCodeQueryValidator : AbstractValidator<GetLanguageByCodeQuery>
{
    public GetLanguageByCodeQueryValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Language.CodeRequired)
            .Length(2).WithErrorCode(ApplicationErrorCodes.Language.InvalidLanguageCode)
            .Matches("^[a-zA-Z]{2}$").WithErrorCode(ApplicationErrorCodes.Language.InvalidLanguageCode);

    }
}
