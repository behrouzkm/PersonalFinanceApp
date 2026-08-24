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

namespace PersonalFinanceApp.WebApi.Controllers;

public class CurrenciesController : BaseApiController
{
      public CurrenciesController(IMediator mediator): base(mediator)
    {
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateCurrencyCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:byte}")]
    public async Task<IActionResult> Update(byte id, UpdateCurrencyCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route currencyId and currencyId must match.");

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:byte}")]
    public async Task<ActionResult> Delete(byte id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCurrencyCommand
        {
            Id = id
        }, cancellationToken);

        return NoContent();
    }


    [HttpGet("{id:byte}")]
    public async Task<ActionResult<Currency>> GetById(byte id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCurrencyByIdQuery
        {
            Id = id
        }, cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<Currency>>> GetList([FromQuery] GetCurrenciesListQuery query,
                        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

}
