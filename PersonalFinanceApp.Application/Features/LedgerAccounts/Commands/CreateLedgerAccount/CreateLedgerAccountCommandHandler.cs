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

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.CreateLedgerAccount;

public class CreateLedgerAccountCommandHandler : IRequestHandler<CreateLedgerAccountCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IOpeningBalanceService _openingBalanceService;
    public CreateLedgerAccountCommandHandler(
                IApplicationDbContext context,
                ICurrentUserService currentUser,
                IOpeningBalanceService openingBalanceService)
    {
        _context = context;
        _currentUser = currentUser;
        _openingBalanceService = openingBalanceService;
    }

    public async Task<Guid> Handle(CreateLedgerAccountCommand request, CancellationToken cancellationToken)
    {
        // all the parentId=null records (roots) have created when tenant register, so parentLedger must have value.
        var parentLedger = await _context.LedgerAccounts
                .FirstOrDefaultAsync(l => l.Id == request.ParentId, cancellationToken)
            ?? throw new NotFoundException(nameof(LedgerAccount), request.ParentId);


        var accountType = await _context.AccountTypes
                .FirstOrDefaultAsync(r=>r.Id == request.AccountTypeId,cancellationToken)
            ?? throw new NotFoundException(nameof(AccountType),request.AccountTypeId);

        var maxDisplayOrder = await _context.LedgerAccounts
                .Where(r => r.ParentId == parentLedger.Id).MaxAsync(c => (int?)c.DisplayOrder, cancellationToken) ?? 0;

        var ledgerAccount = new LedgerAccount(
                request.AccountTypeId,
                request.Name,
                _currentUser.TenantId,
                _currentUser.UserId,
                request.Description
            );

        await _context.LedgerAccounts.AddAsync(ledgerAccount, cancellationToken);
        parentLedger.AddChild(ledgerAccount);

        await _context.SaveChangesAsync(cancellationToken);
        return ledgerAccount.Id;
    }
}
