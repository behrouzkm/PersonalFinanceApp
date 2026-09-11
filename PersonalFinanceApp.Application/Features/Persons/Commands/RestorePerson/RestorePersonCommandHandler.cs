using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.Persons.Commands.RestorePerson;

public class RestorePersonCommandHandler : IRequestHandler<RestorePersonCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IReorderService _reorderService;
    private readonly IAttachmentService _attachmentService;
    private readonly IUnitOfWork _unitOfWork;

    public RestorePersonCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IReorderService reorderService,
        IAttachmentService attachmentService,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _currentUser = currentUser;
        _reorderService = reorderService;
        _attachmentService = attachmentService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RestorePersonCommand request, CancellationToken cancellationToken)
    {
        var person = await _context.Persons
            .IgnoreQueryFilters()
            .Include(p => p.LedgerAccount)
            .Include(p => p.OpeningAccountingDocument)
                .ThenInclude(p => p.Entries)
            .FirstOrDefaultAsync(d => d.Id == request.PersonId
                && d.TenantId == _currentUser.TenantId, cancellationToken)
            ?? throw new NotFoundException(nameof(Person), request.PersonId);

        person.Restore(_currentUser.UserId);
        person.LedgerAccount.Restore(_currentUser.UserId);

        if (person.OpeningAccountingDocument is not null)
            person.OpeningAccountingDocument.Restore(_currentUser.UserId);


        await _reorderService.AppendToEndAsync(person, p => p.TenantId == person.TenantId, cancellationToken);
        await _reorderService.AppendToEndAsync(person.LedgerAccount, p => p.TenantId == person.TenantId &&
                p.ParentId == person.LedgerAccount.ParentId, cancellationToken);

        await _attachmentService.RestoreAllForOwnerAsync(
            AttachmentOwnerType.Person, person.Id, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
