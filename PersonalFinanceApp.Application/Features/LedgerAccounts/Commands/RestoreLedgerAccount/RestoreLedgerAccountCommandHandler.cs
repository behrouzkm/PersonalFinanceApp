using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.RestoreLedgerAccount;

public class RestoreLedgerAccountCommandHandler : IRequestHandler<RestoreLedgerAccountCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IReorderService _reorderService;

    public RestoreLedgerAccountCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser,
        IReorderService reorderService)
    {
        _context = context;
        _currentUser = currentUser;
        _reorderService=reorderService;
    }

    public async Task Handle(RestoreLedgerAccountCommand request, CancellationToken cancellationToken)
    {
        var ledgerAccount = await _context.LedgerAccounts
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == request.LedgerAccountId
                && d.TenantId == _currentUser.TenantId, cancellationToken)
            ?? throw new NotFoundException(nameof(LedgerAccount), request.LedgerAccountId);

        ledgerAccount.Restore(_currentUser.UserId);

        await _reorderService.AppendToEndAsync(ledgerAccount, p => p.TenantId == ledgerAccount.TenantId &&
                p.ParentId == ledgerAccount.ParentId, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
