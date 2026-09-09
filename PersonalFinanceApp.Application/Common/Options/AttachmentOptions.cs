using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Common.Options;

public class AttachmentOptions
{
    public const string SectionName = "Attachments";

    public long MaxFileSizeBytes { get; init; }
    public string[] AllowedContentTypes { get; init; } = [];
}
