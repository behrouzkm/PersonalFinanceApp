using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Persons.Queries.GetPersonById;

public class GetPersonByIdQueryValidator:AbstractValidator<GetPersonByIdQuery>
{
    public GetPersonByIdQueryValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.Person.PersonIdRequired);
    }
}
