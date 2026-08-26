using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.Currencies.Common;

namespace PersonalFinanceApp.Application.Features.Currencies.Queries.GetCurrenciesOptions;

// One flexible query with optional filters, rather than a separate query per filter
// axis - covers listing, date-range reporting, and account/person-based views at once.
public class GetCurrenciesOptionsQuery : IRequest<List<CurrencyOptionDto>>
{
}
