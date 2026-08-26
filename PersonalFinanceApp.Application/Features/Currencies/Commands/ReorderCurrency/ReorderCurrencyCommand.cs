using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.Currencies.Commands.ReorderCurrency;

public class ReorderCurrencyCommand : IRequest
{
    public int Id { get; set; }
    public int NewDisplayOrder { get; set; }
}
