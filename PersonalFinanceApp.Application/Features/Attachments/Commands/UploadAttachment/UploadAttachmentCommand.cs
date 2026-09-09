using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.Attachments.Commands.UploadAttachment;

public record UploadAttachmentCommand : IRequest<Guid>
{
    public AttachmentOwnerType OwnerType { get; init; }
    public Guid OwnerId { get; init; }
    public Stream Content { get; init; } = null!;
    public string FileName { get; init; } = null!;
    public string ContentType { get; init; } = null!;
    public long FileSizeBytes { get; init; }
}
