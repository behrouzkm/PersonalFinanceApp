using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.Incomes.Commands.CreateIncome;
using PersonalFinanceApp.Application.Features.Incomes.Commands.DeleteIncome;
using PersonalFinanceApp.Application.Features.Incomes.Commands.RestoreIncome;
using PersonalFinanceApp.Application.Features.Incomes.Commands.UpdateIncome;
using PersonalFinanceApp.Application.Features.Incomes.Queries.GetIncomeById;
using PersonalFinanceApp.Application.Features.Incomes.Queries.GetIncomesList;

namespace PersonalFinanceApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IncomesController : ControllerBase
{
    private readonly IMediator _mediator;

    public IncomesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateIncomeCommand command, CancellationToken cancellationToken)
    {
        var id = _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateIncomeCommand command, CancellationToken cancellationToken)
    {
        if (id != command.AccountingDocumentId)
            return BadRequest("Route id and command AccountingDocumentId must match.");

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:Guid}")]
    public async Task<ActionResult> Delete(Guid id, [FromQuery] byte[] rowVersion, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteIncomeCommand
        {
            AccountingDocumentId = id,
            RowVersion = rowVersion
        }, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/restore")]
    public async Task<ActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RestoreIncomeCommand
        {
            AccountingDocumentId = id
        }, cancellationToken);

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<IncomeDetailsDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = _mediator.Send(new GetIncomeByIdQuery
        {
            AccountingDocumentId = id
        }, cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<IncomeListItemDto>>> GetList([FromQuery] GetIncomesListQuery query,
                        CancellationToken cancellationToken)
    {
        var result = _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

}
