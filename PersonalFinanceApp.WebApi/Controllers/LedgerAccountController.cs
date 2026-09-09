using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.CreateLedgerAccount;
using PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.DeleteLedgerAccount;
using PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.ReorderLedgerAccount;
using PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.RestoreLedgerAccount;
using PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.UpdateLedgerAccount;
using PersonalFinanceApp.Application.Features.LedgerAccounts.Common;
using PersonalFinanceApp.Application.Features.LedgerAccounts.Queries.GetLedgerAccountById;
using PersonalFinanceApp.Application.Features.LedgerAccounts.Queries.GetLedgerAccountsList;
using PersonalFinanceApp.Application.Features.LedgerAccounts.Queries.GetLedgerAccountsOptions;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.WebApi.Controllers;

public class LedgerAccountController : AttachableApiController
{
    protected override AttachmentOwnerType OwnerType => AttachmentOwnerType.MonetaryAccount;
    public LedgerAccountController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateLedgerAccountCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateLedgerAccountCommand command, CancellationToken cancellationToken)
    {
        if (id != command.LedgerAccountId)
            return BadRequest("Route id and command LedgerAccountId must match.");

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:Guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteLedgerAccountCommand
        {
            LedgerAccountId = id,
        }, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/restore")]
    public async Task<ActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RestoreLedgerAccountCommand
        {
            LedgerAccountId = id
        }, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:guid}/display-order")]
    public async Task<IActionResult> Reorder(Guid id, [FromBody] ReorderLedgerAccountCommand command,
                    CancellationToken cancellationToken)
    {

        if (id != command.LedgerAccountId)
            return BadRequest("Route id and command LedgerAccountId must match.");


        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LedgerAccountDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetLedgerAccountByIdQuery
        {
            LedgerAccountId = id
        }, cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<LedgerAccountDto>>> GetList([FromQuery] GetLedgerAccountsListQuery query,
                        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("options")]
    public async Task<ActionResult<List<LedgerAccountOptionDto>>> GetOptionList([FromQuery] GetLedgerAccountsOptionsQuery query,
                CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

}
