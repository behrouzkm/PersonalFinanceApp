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

namespace PersonalFinanceApp.Application.Features.Persons.Commands.UpdatePerson;

public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IOpeningBalanceService _openingBalanceService;

    public UpdatePersonCommandHandler(
                IApplicationDbContext context,
                ICurrentUserService currentUser,
                IOpeningBalanceService openingBalanceService)
    {
        _context = context;
        _currentUser = currentUser;
        _openingBalanceService = openingBalanceService;
    }

    public async Task Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
    {

        var person = await _context.Persons
        .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
        ?? throw new NotFoundException(nameof(Person), request.Id);

        var oldInitialBalance = person.InitialBalance;
        var oldCreditLimit = person.CreditLimit;
        var oldCurrencyId = person.CurrencyId;
        var existingOpeningDocId = person.OpeningAccountingDocumentId;

        // Mutate first — ReconcileAsync/ValidateAsync read state off this reference,
        // so it must already reflect the requested values before we call it.
        person.UpdateDetails(
            request.PersonType, request.DisplayName, request.CurrencyId, request.OpeningDate,
            request.InitialBalance, _currentUser.UserId, request.CreditLimit,
            request.Email, request.MobileNumber, request.TelNumber, request.Description);

        var openingDocId = await _openingBalanceService.ReconcileAsync(
            person, existingOpeningDocId, oldInitialBalance,oldCreditLimit, oldCurrencyId,
            AccountCategory.PersonAccount, DocumentType.Person, request.Description, cancellationToken);

        person.SetOpeningAccountingDocumentId(openingDocId, _currentUser.UserId);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
