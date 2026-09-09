using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Features.Attachments.Common;

public class AttachmentDto
{
    public Guid Id { get; init; }
    public string FileName { get; init; } = null!;
    public string ContentType { get; init; } = null!;
    public long FileSizeBytes { get; init; }
    public DateTime CreatedAt { get; init; }
}
