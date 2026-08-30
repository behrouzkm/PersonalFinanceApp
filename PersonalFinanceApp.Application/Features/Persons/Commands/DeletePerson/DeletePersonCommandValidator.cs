using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Persons.Commands.DeletePerson;

public class DeletePersonCommandValidator : AbstractValidator<DeletePersonCommand>
{
    public DeletePersonCommandValidator()
    {

        RuleFor(p => p.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.Person.PersonIdRequired);

        
    }
}
