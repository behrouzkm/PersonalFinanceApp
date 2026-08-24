using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.Currencies.Commands.DeleteCurrency;

public class DeleteCurrencyCommand : IRequest
{
    public byte Id { get; set; }

}
