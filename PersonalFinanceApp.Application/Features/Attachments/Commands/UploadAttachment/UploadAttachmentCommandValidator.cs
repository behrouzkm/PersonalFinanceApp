using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Options;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Options;
using PersonalFinanceApp.Domain.Common.Constants;

namespace PersonalFinanceApp.Application.Features.Attachments.Commands.UploadAttachment;

public class UploadAttachmentCommandValidator : AbstractValidator<UploadAttachmentCommand>
{
    public UploadAttachmentCommandValidator(IOptions<AttachmentOptions> options)
    {
        var settings = options.Value;

        RuleFor(x => x.OwnerId).NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.Attachment.OwnerIdRequired);

        RuleFor(x => x.FileName).NotEmpty().MaximumLength(FieldLengths.Name)
            .WithErrorCode(ApplicationErrorCodes.Attachment.FileNameRequired);

        RuleFor(x => x.ContentType)
            .Must(ct => settings.AllowedContentTypes.Contains(ct))
            .WithErrorCode(ApplicationErrorCodes.Attachment.UnsupportedContentType);

        RuleFor(x => x.FileSizeBytes)
            .GreaterThan(0)
            .LessThanOrEqualTo(settings.MaxFileSizeBytes)
            .WithErrorCode(ApplicationErrorCodes.Attachment.FileTooLarge);
    }
}
