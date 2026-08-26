using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.Currencies.Commands.CreateCurrency;
using PersonalFinanceApp.Application.Features.Currencies.Commands.DeleteCurrency;
using PersonalFinanceApp.Application.Features.Currencies.Commands.UpdateCurrency;
using PersonalFinanceApp.Application.Features.Currencies.Queries.GetCurrencyById;
using PersonalFinanceApp.Application.Features.Currencies.Queries.GetCurrenciesList;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Application.Features.Common;
using PersonalFinanceApp.Application.Features.Currencies.Common;
using PersonalFinanceApp.Application.Features.Currencies.Queries.GetCurrenciesOptions;

namespace PersonalFinanceApp.WebApi.Controllers;

public class CurrenciesController : BaseApiController
{
      public CurrenciesController(IMediator mediator): base(mediator)
    {
    }



    [HttpGet]
    public async Task<ActionResult<List<CurrencyOptionDto>>> GetOptionList([FromQuery] GetCurrenciesOptionsQuery query,
                        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

}
