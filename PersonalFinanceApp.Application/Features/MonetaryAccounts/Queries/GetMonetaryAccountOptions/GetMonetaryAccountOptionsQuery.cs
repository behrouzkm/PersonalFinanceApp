using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Features.Common;

namespace PersonalFinanceApp.Application.Features.MonetaryAccounts.Queries.GetMonetaryAccountOptions;

public class GetMonetaryAccountOptionsQuery : IRequest<List<MonetaryAccountOptionDto>>
{

}
