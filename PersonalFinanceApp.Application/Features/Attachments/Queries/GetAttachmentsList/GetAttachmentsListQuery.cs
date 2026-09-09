using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Features.Attachments.Common;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.Attachments.Queries.GetAttachmentsList;

public record GetAttachmentsListQuery(AttachmentOwnerType OwnerType, Guid OwnerId)
    : IRequest<IReadOnlyList<AttachmentDto>>;
