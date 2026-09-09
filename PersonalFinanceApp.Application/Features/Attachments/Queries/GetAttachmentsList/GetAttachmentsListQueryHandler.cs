using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Features.Attachments.Common;

namespace PersonalFinanceApp.Application.Features.Attachments.Queries.GetAttachmentsList;

public class GetAttachmentsListQueryHandler
    : IRequestHandler<GetAttachmentsListQuery, IReadOnlyList<AttachmentDto>>
{
    private readonly IAttachmentService _attachmentService;

    public GetAttachmentsListQueryHandler(IAttachmentService attachmentService) => _attachmentService = attachmentService;

    public Task<IReadOnlyList<AttachmentDto>> Handle(GetAttachmentsListQuery request, CancellationToken cancellationToken) =>
        _attachmentService.GetForOwnerAsync(request.OwnerType, request.OwnerId, cancellationToken);
}
