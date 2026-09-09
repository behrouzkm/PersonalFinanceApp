using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.Persons.Commands.CreatePerson;
using PersonalFinanceApp.Application.Features.Persons.Commands.DeletePerson;
using PersonalFinanceApp.Application.Features.Persons.Commands.ReorderPerson;
using PersonalFinanceApp.Application.Features.Persons.Commands.RestorePerson;
using PersonalFinanceApp.Application.Features.Persons.Commands.UpdatePerson;
using PersonalFinanceApp.Application.Features.Persons.Common;
using PersonalFinanceApp.Application.Features.Persons.Queries.GetPersonById;
using PersonalFinanceApp.Application.Features.Persons.Queries.GetPersonsList;
using PersonalFinanceApp.Application.Features.Persons.Queries.GetPersonsOptions;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.WebApi.Controllers;


public class PersonController : AttachableApiController
{
    protected override AttachmentOwnerType OwnerType => AttachmentOwnerType.Person;

    public PersonController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreatePersonCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdatePersonCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route id and command PersonId must match.");

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:Guid}")]
    public async Task<ActionResult> Delete(Guid id, [FromQuery] byte[] rowVersion, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeletePersonCommand
        {
            PersonId = id,
            RowVersion = rowVersion
        }, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/restore")]
    public async Task<ActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RestorePersonCommand
        {
            PersonId = id
        }, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:guid}/display-order")]
    public async Task<IActionResult> Reorder(Guid id, [FromBody] ReorderPersonCommand command,
                        CancellationToken cancellationToken)
    {

        if (id != command.Id)
            return BadRequest("Route id and command PersonId must match.");


        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PersonDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPersonByIdQuery
        {
            PersonId = id
        }, cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<PersonDto>>> GetList([FromQuery] GetPersonsListQuery query,
                        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("options")]
    public async Task<ActionResult<List<PersonOptionDto>>> GetOptionList([FromQuery] GetPersonsOptionsQuery query,
                    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }
}

