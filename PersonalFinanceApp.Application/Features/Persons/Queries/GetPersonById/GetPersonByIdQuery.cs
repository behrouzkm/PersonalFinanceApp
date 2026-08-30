using MediatR;
using PersonalFinanceApp.Application.Features.Persons.Common;


namespace PersonalFinanceApp.Application.Features.Persons.Queries.GetPersonById;

public class GetPersonByIdQuery : IRequest<PersonDto>
{
    public Guid Id {get;set;}
}
