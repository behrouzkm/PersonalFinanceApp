using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Application.Features.Attachments.Commands.DeleteAttachment;
using PersonalFinanceApp.Application.Features.Attachments.Commands.UploadAttachment;
using PersonalFinanceApp.Application.Features.Attachments.Common;
using PersonalFinanceApp.Application.Features.Attachments.Queries.GetAttachmentDownload;
using PersonalFinanceApp.Application.Features.Attachments.Queries.GetAttachmentsList;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.WebApi.Controllers;

[Route("api/[controller]")]
public class AttachmentsController : BaseApiController
{
    public AttachmentsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [RequestSizeLimit(50 * 1024 * 1024)]   // hard infra ceiling — rarely changes
    public async Task<ActionResult<Guid>> Upload(
        [FromForm] AttachmentOwnerType ownerType, [FromForm] Guid ownerId, [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        var command = new UploadAttachmentCommand
        {
            OwnerType = ownerType,
            OwnerId = ownerId,
            Content = stream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSizeBytes = file.Length
        };
        var id = await _mediator.Send(command, cancellationToken);
        return Ok(id);
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAttachmentDownloadQuery(id), cancellationToken);
        return File(result.Content, result.ContentType, result.FileName);
    }


    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AttachmentDto>>> GetList(
        [FromQuery] AttachmentOwnerType ownerType, [FromQuery] Guid ownerId, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetAttachmentsListQuery(ownerType, ownerId), cancellationToken));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteAttachmentCommand(id), cancellationToken);
        return NoContent();
    }
}
