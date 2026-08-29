using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.Currencies.Common;

namespace PersonalFinanceApp.Application.Features.Currencies.Queries.GetCurrenciesOptions;

public class GetCurrenciesOptionsQuery : IRequest<List<CurrencyOptionDto>>
{
}
