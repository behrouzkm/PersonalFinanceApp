using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.Currencies.Queries.GetCurrencyById;

public class GetCurrencyByIdQuery : IRequest<Currency>
{
    public byte Id {get;set;}
}
