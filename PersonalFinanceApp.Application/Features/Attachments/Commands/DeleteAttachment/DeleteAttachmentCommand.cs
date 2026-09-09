using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.Attachments.Commands.DeleteAttachment;

public record DeleteAttachmentCommand(Guid Id) : IRequest;
