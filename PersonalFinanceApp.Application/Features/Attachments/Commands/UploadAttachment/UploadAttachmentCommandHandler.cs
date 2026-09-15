using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;

namespace PersonalFinanceApp.Application.Features.Attachments.Commands.UploadAttachment;

public class UploadAttachmentCommandHandler
{

    private readonly IAttachmentService _attachmentService;
    private readonly IFileContentValidator _fileContentValidator;

    public UploadAttachmentCommandHandler(IAttachmentService attachmentService,
            IFileContentValidator fileContentValidator)
    {
        _attachmentService = attachmentService;
        _fileContentValidator = fileContentValidator;
    }

    public async Task<Guid> Handle(UploadAttachmentCommand request, CancellationToken cancellationToken)
    {
        if (!await _fileContentValidator.IsValidAsync(
            request.Content,
            request.ContentType,
            cancellationToken))
        {
            throw new BusinessRuleException(
                ApplicationErrorCodes.Attachment.InvalidFileContent);
        }

        return await _attachmentService.UploadAsync(request.OwnerType, request.OwnerId, request.Content,
            request.FileName, request.ContentType, request.FileSizeBytes, cancellationToken);
    }
}
