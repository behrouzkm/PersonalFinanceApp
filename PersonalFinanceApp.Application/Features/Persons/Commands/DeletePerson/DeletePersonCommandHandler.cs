using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.Persons.Commands.DeletePerson;

public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IReorderService _reorderService;
    private readonly IAttachmentService _attachmentService;

    public DeletePersonCommandHandler(
                IApplicationDbContext context,
                ICurrentUserService currentUser,
                IReorderService reorderService,
                IAttachmentService attachmentService)
    {
        _context = context;
        _currentUser = currentUser;
        _reorderService = reorderService;
        _attachmentService = attachmentService;
    }

    public async Task Handle(DeletePersonCommand request, CancellationToken cancellationToken)
    {

        var person = await _context.Persons
        .Include(r => r.LedgerAccount)
        .Include(r => r.OpeningAccountingDocument)
            .ThenInclude(d => d!.Entries)
        .FirstOrDefaultAsync(r => r.Id == request.PersonId, cancellationToken)
        ?? throw new NotFoundException(nameof(Person), request.PersonId);

        _context.Entry(person).Property(d => d.RowVersion).OriginalValue = request.RowVersion;

        var hasAccountingHistory = await _context.AccountingEntries
            .AnyAsync(r => r.AccountingDocumentId != person.OpeningAccountingDocumentId &&
                 r.LedgerAccountId == person.LedgerAccountId, cancellationToken);

        if (hasAccountingHistory)
            throw new BusinessRuleException(ApplicationErrorCodes.Person.CannotDeleteWithAccountingHistory);

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Phase 1: commit the deletion first — this is what actually vacates
            // the slot under the filtered unique index.
            if (person.OpeningAccountingDocumentId is not null)
                person.OpeningAccountingDocument!.SoftDelete(_currentUser.UserId);

            person.LedgerAccount.SoftDelete(_currentUser.UserId);
            person.SoftDelete(_currentUser.UserId);
            await _context.SaveChangesAsync(cancellationToken);

            // Phase 2: now the vacated slot is genuinely free at the DB level —
            // safe to shift survivors into it regardless of statement order.
            await _reorderService.CloseGapAsync(person, cancellationToken,p => p.TenantId == person.TenantId);
            await _reorderService.CloseGapAsync(person.LedgerAccount, cancellationToken,
                p => p.TenantId == person.TenantId && p.ParentId == person.LedgerAccount.ParentId);
            await _context.SaveChangesAsync(cancellationToken);

            await _attachmentService.SoftDeleteAllForOwnerAsync(
                AttachmentOwnerType.Person, person.Id, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
