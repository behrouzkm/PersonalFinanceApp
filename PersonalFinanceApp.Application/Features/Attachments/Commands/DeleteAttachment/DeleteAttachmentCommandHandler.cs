using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Common.Interfaces;

namespace PersonalFinanceApp.Application.Features.Attachments.Commands.DeleteAttachment;

public class DeleteAttachmentCommandHandler : IRequestHandler<DeleteAttachmentCommand>
{
    private readonly IAttachmentService _attachmentService;

    public DeleteAttachmentCommandHandler(IAttachmentService attachmentService) => _attachmentService = attachmentService;

    public Task Handle(DeleteAttachmentCommand request, CancellationToken cancellationToken) =>
        _attachmentService.DeleteAsync(request.Id, cancellationToken);
}
