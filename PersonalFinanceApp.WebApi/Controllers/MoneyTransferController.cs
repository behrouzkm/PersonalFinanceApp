using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.MoneyTransfers.Commands.CreateMoneyTransfer;
using PersonalFinanceApp.Application.Features.MoneyTransfers.Commands.DeleteMoneyTransfer;
using PersonalFinanceApp.Application.Features.MoneyTransfers.Commands.RestoreMoneyTransfer;
using PersonalFinanceApp.Application.Features.MoneyTransfers.Commands.UpdateMoneyTransfer;
using PersonalFinanceApp.Application.Features.MoneyTransfers.Common;
using PersonalFinanceApp.Application.Features.MoneyTransfers.Queries.GetMoneyTransferById;
using PersonalFinanceApp.Application.Features.MoneyTransfers.Queries.GetMoneyTransfersList;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.WebApi.Controllers;

public class MoneyTransferController : AttachableApiController
{
    protected override AttachmentOwnerType OwnerType => AttachmentOwnerType.AccountingDocument;
    public MoneyTransferController(IMediator mediator): base(mediator)
    {
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateMoneyTransferCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateMoneyTransferCommand command, CancellationToken cancellationToken)
    {
        if (id != command.MoneyTransferDocumentId)
            return BadRequest("Route id and command MoneyTransferDocumentId must match.");

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:Guid}")]
    public async Task<ActionResult> Delete(Guid id, [FromQuery] byte[] rowVersion, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteMoneyTransferCommand
        {
            MoneyTransferDocumentId = id,
            RowVersion = rowVersion
        }, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/restore")]
    public async Task<ActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RestoreMoneyTransferCommand
        {
            MoneyTransferDocumentId = id
        }, cancellationToken);

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MoneyTransferDetailsDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMoneyTransferByIdQuery
        {
            MoneyTransferDocumentId = id
        }, cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<MoneyTransferListItemDto>>> GetList([FromQuery] GetMoneyTransfersListQuery query,
                        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

}
