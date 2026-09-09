using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.CurrencyExchanges.Common;

namespace PersonalFinanceApp.Application.Features.CurrencyExchanges.Queries.GetCurrencyExchangeById;

public class GetCurrencyExchangeByIdQuery : IRequest<CurrencyExchangeDto>
{
    public Guid CurrencyExchangeId { get; set; }
}
