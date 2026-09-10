using MediatR;
using PersonalFinanceApp.Application.Features.AccountTypeTranslations.Common;


namespace PersonalFinanceApp.Application.Features.AccountTypeTranslations.Queries.GetAccountTypeTranslationById;

public class GetAccountTypeTranslationByIdQuery : IRequest<AccountTypeTranslationDto>
{
    public int Id {get;set;}
}
