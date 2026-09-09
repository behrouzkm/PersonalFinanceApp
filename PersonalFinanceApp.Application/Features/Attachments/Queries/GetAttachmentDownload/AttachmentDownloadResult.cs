using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Features.Attachments.Queries.GetAttachmentDownload;

public class AttachmentDownloadResult
{
    public Stream Content { get; init; } = null!;
    public string ContentType { get; init; } = null!;
    public string FileName { get; init; } = null!;
}
