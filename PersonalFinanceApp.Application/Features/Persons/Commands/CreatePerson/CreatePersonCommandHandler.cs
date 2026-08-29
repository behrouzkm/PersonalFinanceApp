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

namespace PersonalFinanceApp.Application.Features.Persons.Commands.CreatePerson;

public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IOpeningBalanceService _openingBalanceService;
    public CreatePersonCommandHandler(
                IApplicationDbContext context,
                ICurrentUserService currentUser,
                IOpeningBalanceService openingBalanceService)
    {
        _context = context;
        _currentUser = currentUser;
        _openingBalanceService = openingBalanceService;
    }

    public async Task<Guid> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        var (ledgerAccount, openingDocId) = await _openingBalanceService.CreateAsync(
        request.ParentLedgerId, AccountCategory.PersonAccount, DocumentType.Person,
        request.DisplayName, request.OpeningDate, request.CurrencyId,
        request.InitialBalance, request.CreditLimit, request.Description, cancellationToken);

        var maxDisplayOrder = await _context.Persons.MaxAsync(c => (int?)c.DisplayOrder, cancellationToken) ?? 0;

        var person = new Person(
            request.PersonType, request.DisplayName, ledgerAccount.Id, request.CurrencyId,
            maxDisplayOrder + 1, request.OpeningDate, request.InitialBalance,
            _currentUser.TenantId, _currentUser.UserId,
            request.Email, request.MobileNumber, request.TelNumber, request.Description,
            request.CreditLimit, openingDocId);

        await _context.Persons.AddAsync(person, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return person.Id;
    }
}
