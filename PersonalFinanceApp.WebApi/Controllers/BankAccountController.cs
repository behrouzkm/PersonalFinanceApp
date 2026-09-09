using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.BankAccounts.Commands.CreateBankAccount;
using PersonalFinanceApp.Application.Features.BankAccounts.Commands.DeleteBankAccount;
using PersonalFinanceApp.Application.Features.BankAccounts.Commands.ReorderBankAccount;
using PersonalFinanceApp.Application.Features.BankAccounts.Commands.RestoreBankAccount;
using PersonalFinanceApp.Application.Features.BankAccounts.Commands.UpdateBankAccount;
using PersonalFinanceApp.Application.Features.BankAccounts.Common;
using PersonalFinanceApp.Application.Features.BankAccounts.Queries.GetBankAccountById;
using PersonalFinanceApp.Application.Features.BankAccounts.Queries.GetBankAccountsList;
using PersonalFinanceApp.Application.Features.BankAccounts.Queries.GetBankAccountsOptions;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.WebApi.Controllers;

public class BankAccountController : AttachableApiController
{
    protected override AttachmentOwnerType OwnerType => AttachmentOwnerType.MonetaryAccount;
    public BankAccountController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateBankAccountCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateBankAccountCommand command, CancellationToken cancellationToken)
    {
        if (id != command.BankAccountId)
            return BadRequest("Route id and command BankAccountId must match.");

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:Guid}")]
    public async Task<ActionResult> Delete(Guid id, [FromQuery] byte[] rowVersion, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteBankAccountCommand
        {
            BankAccountId = id,
            RowVersion = rowVersion
        }, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/restore")]
    public async Task<ActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RestoreBankAccountCommand
        {
            BankAccountId = id
        }, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:int}/display-order")]
    public async Task<IActionResult> Reorder(Guid id, [FromBody] ReorderBankAccountCommand command,
                    CancellationToken cancellationToken)
    {

        if (id != command.BankAccountId)
            return BadRequest("Route id and command BankAccountId must match.");


        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BankAccountDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBankAccountByIdQuery
        {
            BankAccountId = id
        }, cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<BankAccountListItemDto>>> GetList([FromQuery] GetBankAccountsListQuery query,
                        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<BankAccountOptionDto>>> GetOptionList([FromQuery] GetBankAccountsOptionsQuery query,
                CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

}
