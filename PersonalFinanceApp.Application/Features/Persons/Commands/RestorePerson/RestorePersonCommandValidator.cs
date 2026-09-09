using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Persons.Commands.RestorePerson;

public class RestorePersonCommandValidator : AbstractValidator<RestorePersonCommand>
{
    public RestorePersonCommandValidator()
    {
        RuleFor(x=> x.PersonId)
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.Person.PersonIdRequired);
    }
}
