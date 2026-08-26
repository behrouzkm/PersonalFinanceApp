using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Features.Currencies.Common;


namespace PersonalFinanceApp.Application.Features.Currencies.Queries.GetCurrencyById;

public class GetCurrencyByIdQuery : IRequest<CurrencyDto>
{
    public int Id {get;set;}
}
