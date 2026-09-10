using MediatR;
using PersonalFinanceApp.Application.Features.Currencies.Common;


namespace PersonalFinanceApp.Application.Features.Currencies.Queries.GetCurrencyByCode;

public class GetCurrencyByCodeQuery : IRequest<CurrencyDto>
{
    public string Code {get;set;}=string.Empty;
}
