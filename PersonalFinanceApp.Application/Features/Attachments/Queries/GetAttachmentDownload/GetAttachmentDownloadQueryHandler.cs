using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Common.Interfaces;

namespace PersonalFinanceApp.Application.Features.Attachments.Queries.GetAttachmentDownload;

public class GetAttachmentDownloadQueryHandler : IRequestHandler<GetAttachmentDownloadQuery, AttachmentDownloadResult>
{
    private readonly IAttachmentService _attachmentService;

    public GetAttachmentDownloadQueryHandler(IAttachmentService attachmentService) => _attachmentService = attachmentService;

    public async Task<AttachmentDownloadResult> Handle(GetAttachmentDownloadQuery request, CancellationToken cancellationToken)
    {
        var (content, contentType, fileName) = await _attachmentService.DownloadAsync(request.Id, cancellationToken);
        return new AttachmentDownloadResult { Content = content, ContentType = contentType, FileName = fileName };
    }
}
