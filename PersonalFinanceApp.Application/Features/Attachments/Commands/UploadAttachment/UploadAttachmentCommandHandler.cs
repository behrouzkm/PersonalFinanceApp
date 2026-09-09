using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Common.Interfaces;

namespace PersonalFinanceApp.Application.Features.Attachments.Commands.UploadAttachment;

public class UploadAttachmentCommandHandler
{

    private readonly IAttachmentService _attachmentService;

    public UploadAttachmentCommandHandler(IAttachmentService attachmentService) => _attachmentService = attachmentService;

    public Task<Guid> Handle(UploadAttachmentCommand request, CancellationToken cancellationToken) =>
        _attachmentService.UploadAsync(request.OwnerType, request.OwnerId, request.Content,
            request.FileName, request.ContentType, request.FileSizeBytes, cancellationToken);
}
