using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.Languages.Commands.CreateLanguage;
using PersonalFinanceApp.Application.Features.Languages.Commands.DeleteLanguage;
using PersonalFinanceApp.Application.Features.Languages.Commands.UpdateLanguage;
using PersonalFinanceApp.Application.Features.Languages.Queries.GetLanguageById;
using PersonalFinanceApp.Application.Features.Languages.Queries.GetLanguagesList;
using PersonalFinanceApp.Application.Features.Languages.Common;
using PersonalFinanceApp.Application.Features.Languages.Commands.ReorderLanguage;

namespace PersonalFinanceApp.WebApi.Controllers.Admin;

public class LanguagesController : AdminBaseApiController
{
    public LanguagesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateLanguageCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateLanguageCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route id and command LanguageId must match.");

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteLanguageCommand
        {
            Id = id
        }, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:int}/display-order")]
    public async Task<IActionResult> Reorder(int id, [FromBody] ReorderLanguageCommand command,
                        CancellationToken cancellationToken)
    {

        if (id != command.Id)
            return BadRequest("Route id and command LanguageId must match.");


        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LanguageDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetLanguageByIdQuery
        {
            Id = id
        }, cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<LanguageDto>>> GetList([FromQuery] GetLanguagesListQuery query,
                        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

}
