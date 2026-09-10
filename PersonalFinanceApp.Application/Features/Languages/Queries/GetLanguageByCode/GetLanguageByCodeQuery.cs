using MediatR;
using PersonalFinanceApp.Application.Features.Languages.Common;


namespace PersonalFinanceApp.Application.Features.Languages.Queries.GetLanguageByCode;

public class GetLanguageByCodeQuery : IRequest<LanguageDto>
{
    public string Code {get;set;}=string.Empty;
}
