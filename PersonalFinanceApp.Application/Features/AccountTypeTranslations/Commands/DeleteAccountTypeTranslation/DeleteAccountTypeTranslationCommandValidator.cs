using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.AccountTypeTranslations.Commands.DeleteAccountTypeTranslation;

public class DeleteAccountTypeTranslationCommandValidator : AbstractValidator<DeleteAccountTypeTranslationCommand>
{
    public DeleteAccountTypeTranslationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.AccountTypeTranslation.AccountTypeTranslationIdRequired);
    }
}
