using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.CashAccounts.Commands.CreateCashAccount;
using PersonalFinanceApp.Application.Features.CashAccounts.Commands.DeleteCashAccount;
using PersonalFinanceApp.Application.Features.CashAccounts.Commands.ReorderCashAccount;
using PersonalFinanceApp.Application.Features.CashAccounts.Commands.RestoreCashAccount;
using PersonalFinanceApp.Application.Features.CashAccounts.Commands.UpdateCashAccount;
using PersonalFinanceApp.Application.Features.CashAccounts.Common;
using PersonalFinanceApp.Application.Features.CashAccounts.Queries.GetCashAccountById;
using PersonalFinanceApp.Application.Features.CashAccounts.Queries.GetCashAccountsList;
using PersonalFinanceApp.Application.Features.CashAccounts.Queries.GetCashAccountsOptions;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.WebApi.Controllers;

public class CashAccountController : AttachableApiController
{
    protected override AttachmentOwnerType OwnerType => AttachmentOwnerType.MonetaryAccount;
    public CashAccountController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateCashAccountCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCashAccountCommand command, CancellationToken cancellationToken)
    {
        if (id != command.CashAccountId)
            return BadRequest("Route id and command CashAccountId must match.");

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:Guid}")]
    public async Task<ActionResult> Delete(Guid id, [FromQuery] byte[] rowVersion, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCashAccountCommand
        {
            CashAccountId = id,
            RowVersion = rowVersion
        }, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/restore")]
    public async Task<ActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RestoreCashAccountCommand
        {
            CashAccountId = id
        }, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:guid}/display-order")]
    public async Task<IActionResult> Reorder(Guid id, [FromBody] ReorderCashAccountCommand command,
                    CancellationToken cancellationToken)
    {

        if (id != command.CashAccountId)
            return BadRequest("Route id and command CashAccountId must match.");


        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CashAccountDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCashAccountByIdQuery
        {
            CashAccountId = id
        }, cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<CashAccountListItemDto>>> GetList([FromQuery] GetCashAccountsListQuery query,
                        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("options")]
    public async Task<ActionResult<List<CashAccountOptionDto>>> GetOptionList([FromQuery] GetCashAccountsOptionsQuery query,
                CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

}
