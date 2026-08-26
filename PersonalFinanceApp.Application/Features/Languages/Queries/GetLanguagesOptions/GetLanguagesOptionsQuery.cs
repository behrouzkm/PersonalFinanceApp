using MediatR;
using PersonalFinanceApp.Application.Features.Languages.Common;

namespace PersonalFinanceApp.Application.Features.Languages.Queries.GetLanguagesOptions;

// One flexible query with optional filters, rather than a separate query per filter
// axis - covers listing, date-range reporting, and account/person-based views at once.
public class GetLanguagesOptionsQuery : IRequest<List<LanguageOptionDto>>
{
}
