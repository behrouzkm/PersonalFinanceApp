using MediatR;
using PersonalFinanceApp.Application.Features.Languages.Common;


namespace PersonalFinanceApp.Application.Features.Languages.Queries.GetLanguageById;

public class GetLanguageByIdQuery : IRequest<LanguageDto>
{
    public int Id {get;set;}
}
