using MediatR;
using PersonalFinanceApp.Application.Features.Persons.Common;

namespace PersonalFinanceApp.Application.Features.Persons.Queries.GetPersonsOptions;

// One flexible query with optional filters, rather than a separate query per filter
// axis - covers listing, date-range reporting, and account/person-based views at once.
public class GetPersonsOptionsQuery : IRequest<List<PersonOptionDto>>
{
}
