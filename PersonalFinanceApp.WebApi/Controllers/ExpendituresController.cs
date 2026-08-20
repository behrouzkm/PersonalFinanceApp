using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.Expenditures.Commands.CreateExpenditure;
using PersonalFinanceApp.Application.Features.Expenditures.Commands.DeleteExpenditure;
using PersonalFinanceApp.Application.Features.Expenditures.Commands.RestoreExpenditure;
using PersonalFinanceApp.Application.Features.Expenditures.Commands.UpdateExpenditure;
using PersonalFinanceApp.Application.Features.Expenditures.Queries.GetExpenditureById;
using PersonalFinanceApp.Application.Features.Expenditures.Queries.GetExpenditureByList;

namespace PersonalFinanceApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpendituresController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExpendituresController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateExpenditureCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateExpenditureCommand command, CancellationToken cancellationToken)
    {
        if (id != command.AccountingDocumentId)
            return BadRequest("Route id and command AccountingDocumentId must match.");

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] byte[] rowVersion, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteExpenditureCommand
        {
            AccountingDocumentId = id,
            RowVersion = rowVersion
        },
            cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RestoreExpenditureCommand
        {
            AccountingDocumentId = id
        }, cancellationToken);

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExpenditureDetailsDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetExpenditureByIdQuery
        {
            AccountingDocumentId = id
        }, cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<ExpenditureListItemDto>>> GetList([FromQuery] GetExpendituresListQuery query,
                        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
