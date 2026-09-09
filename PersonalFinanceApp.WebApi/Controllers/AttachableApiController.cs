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

public abstract class AttachableApiController : BaseApiController
{
    protected abstract AttachmentOwnerType OwnerType { get; }

    protected AttachableApiController(IMediator mediator) : base(mediator) { }

    [HttpPost("{id:guid}/attachments")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    public async Task<ActionResult<Guid>> UploadAttachment(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        var attachmentId = await _mediator.Send(new UploadAttachmentCommand
        {
            OwnerType = OwnerType,
            OwnerId = id,
            Content = stream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSizeBytes = file.Length
        }, cancellationToken);

        return Ok(attachmentId);
    }

    [HttpGet("{id:guid}/attachments")]
    public async Task<ActionResult<IReadOnlyList<AttachmentDto>>> GetAttachments(Guid id, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetAttachmentsListQuery(OwnerType, id), cancellationToken));

    [HttpDelete("{id:guid}/attachments/{attachmentId:guid}")]
    public async Task<IActionResult> DeleteAttachment(Guid id, Guid attachmentId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteAttachmentCommand(attachmentId), cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}/attachments/{attachmentId:guid}/download")]
    public async Task<IActionResult> DownloadAttachment(Guid id, Guid attachmentId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAttachmentDownloadQuery(attachmentId), cancellationToken);
        return File(result.Content, result.ContentType, result.FileName);
    }
}
