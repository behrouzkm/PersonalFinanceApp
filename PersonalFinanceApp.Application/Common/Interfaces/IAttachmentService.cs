using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Features.Attachments.Common;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Common.Interfaces;

public interface IAttachmentService
{

    Task<Guid> UploadAsync(AttachmentOwnerType ownerType, Guid ownerId, Stream content, string fileName,
            string contentType, long fileSizeBytes, CancellationToken cancellationToken);

    Task<(Stream Content, string ContentType, string FileName)> DownloadAsync(
            Guid attachmentId, CancellationToken cancellationToken);

    Task<IReadOnlyList<AttachmentDto>> GetForOwnerAsync(AttachmentOwnerType ownerType, Guid ownerId,
        CancellationToken cancellationToken);

    Task DeleteAsync(Guid attachmentId, CancellationToken cancellationToken);

    // Called from the owner's own Delete/Restore handler — same principle as
    // ReorderService.CloseGapAsync being wired into DeletePersonCommandHandler.
    Task SoftDeleteAllForOwnerAsync(AttachmentOwnerType ownerType, Guid ownerId, CancellationToken cancellationToken);
    Task RestoreAllForOwnerAsync(AttachmentOwnerType ownerType, Guid ownerId, CancellationToken cancellationToken);
}
