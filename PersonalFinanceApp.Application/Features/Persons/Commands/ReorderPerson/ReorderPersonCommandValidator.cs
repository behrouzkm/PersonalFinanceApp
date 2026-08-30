using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Persons.Commands.ReorderPerson;

public class ReorderPersonCommandValidator : AbstractValidator<ReorderPersonCommand>
{
    public ReorderPersonCommandValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.Person.PersonIdRequired);

        RuleFor(x => x.NewDisplayOrder)
            .GreaterThan(0).WithErrorCode(ApplicationErrorCodes.Person.InvalidDisplayOrder);
    }
}
