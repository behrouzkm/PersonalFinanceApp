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
    private readonly IOpeningBalanceService _openingBalanceService;

    public DeletePersonCommandHandler(
                IApplicationDbContext context,
                ICurrentUserService currentUser,
                IOpeningBalanceService openingBalanceService)
    {
        _context = context;
        _currentUser = currentUser;
        _openingBalanceService = openingBalanceService;
    }

    public async Task Handle(DeletePersonCommand request, CancellationToken cancellationToken)
    {

        var person = await _context.Persons
            .Include(r => r.LedgerAccount)
            .Include(r => r.OpeningAccountingDocument)
                .ThenInclude(d=>d.Entries)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Person), request.Id);

        var hasAccountingHistory = await _context.AccountingEntries
            .AnyAsync(r => r.AccountingDocumentId != person.OpeningAccountingDocumentId &&
                 r.LedgerAccountId == person.LedgerAccountId, cancellationToken);


        if (hasAccountingHistory)
            throw new BusinessRuleException(ApplicationErrorCodes.Person.CannotDeleteWithAccountingHistory);

        if (person.OpeningAccountingDocumentId is not null)
            person.OpeningAccountingDocument!.SoftDelete(_currentUser.UserId);

        person.LedgerAccount.SoftDelete(_currentUser.UserId);
        person.SoftDelete(_currentUser.UserId);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
