using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.CurrencyExchanges.Commands.CreateCurrencyExchange;
using PersonalFinanceApp.Application.Features.CurrencyExchanges.Commands.DeleteCurrencyExchange;
using PersonalFinanceApp.Application.Features.CurrencyExchanges.Commands.RestoreCurrencyExchange;
using PersonalFinanceApp.Application.Features.CurrencyExchanges.Commands.UpdateCurrencyExchange;
using PersonalFinanceApp.Application.Features.CurrencyExchanges.Common;
using PersonalFinanceApp.Application.Features.CurrencyExchanges.Queries.GetCurrencyExchangeById;
using PersonalFinanceApp.Application.Features.CurrencyExchanges.Queries.GetCurrencyExchangesList;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.WebApi.Controllers;

public class CurrencyExchangeController : AttachableApiController
{
    protected override AttachmentOwnerType OwnerType => AttachmentOwnerType.CurrencyExchange;
    public CurrencyExchangeController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateCurrencyExchangeCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCurrencyExchangeCommand command, CancellationToken cancellationToken)
    {
        if (id != command.CurrencyExchangeId)
            return BadRequest("Route id and command CurrencyExchangeId must match.");

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:Guid}")]
    public async Task<ActionResult> Delete(Guid id, [FromQuery] byte[] rowVersion, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCurrencyExchangeCommand
        {
            CurrencyExchangeId = id,
            RowVersion = rowVersion
        }, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/restore")]
    public async Task<ActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RestoreCurrencyExchangeCommand
        {
            CurrencyExchangeId = id
        }, cancellationToken);

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CurrencyExchangeDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCurrencyExchangeByIdQuery
        {
            CurrencyExchangeId = id
        }, cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<CurrencyExchangeListItemDto>>> GetList([FromQuery] GetCurrencyExchangesListQuery query,
                        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

}
