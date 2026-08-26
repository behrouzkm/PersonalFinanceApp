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
using PersonalFinanceApp.Application.Features.Currencies.Commands.ReorderCurrency;

namespace PersonalFinanceApp.WebApi.Controllers.Admin;

public class CurrenciesController : AdminBaseApiController
{
    public CurrenciesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateCurrencyCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateCurrencyCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route id and command CurrencyId must match.");

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCurrencyCommand
        {
            Id = id
        }, cancellationToken);

        return NoContent();
    }


    [HttpPatch("{id:int}/display-order")]
    public async Task<IActionResult> Reorder(int id, [FromBody] ReorderCurrencyCommand command,
                        CancellationToken cancellationToken)
    {

        if (id != command.Id)
            return BadRequest("Route id and command CurrencyId must match.");


        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CurrencyDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCurrencyByIdQuery
        {
            Id = id
        }, cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<CurrencyDto>>> GetList([FromQuery] GetCurrenciesListQuery query,
                        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

}
