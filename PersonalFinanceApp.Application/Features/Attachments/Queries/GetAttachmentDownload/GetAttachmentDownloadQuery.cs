using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.Attachments.Queries.GetAttachmentDownload;

public record GetAttachmentDownloadQuery(Guid Id) : IRequest<AttachmentDownloadResult>;

