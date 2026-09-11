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

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.DeleteLedgerAccount;

public class DeleteLedgerAccountCommandHandler : IRequestHandler<DeleteLedgerAccountCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IReorderService _reorderService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransactionManager _transactionManager;

    public DeleteLedgerAccountCommandHandler(
                IApplicationDbContext context,
                ICurrentUserService currentUser,
                IReorderService reorderService,
                IUnitOfWork unitOfWork,
                ITransactionManager transactionManager)
    {
        _context = context;
        _currentUser = currentUser;
        _reorderService = reorderService;
        _unitOfWork = unitOfWork;
        _transactionManager = transactionManager;
    }

    public async Task Handle(DeleteLedgerAccountCommand request, CancellationToken cancellationToken)
    {
        var ledgerAccount = await _context.LedgerAccounts
                .Include(l=>l.AccountType)
                .FirstOrDefaultAsync(r => r.Id == request.LedgerAccountId, cancellationToken)
                ?? throw new NotFoundException(nameof(LedgerAccount), request.LedgerAccountId);

        // Checking if ledger account is not Income or Expenditure
        // because other type must delete from their own entity like Person
        if(!ledgerAccount.AccountType.CanDeleteDirectly)
            throw new BusinessRuleException(ApplicationErrorCodes.LedgerAccount.CannotDeleteDirectly);

        //_context.Entry(ledgerAccount).Property(d => d.RowVersion).OriginalValue = request.RowVersion;

        var hasAccountingHistory = await _context.AccountingEntries
            .AnyAsync(r => r.LedgerAccountId == ledgerAccount.Id, cancellationToken);

        if (hasAccountingHistory)
            throw new BusinessRuleException(ApplicationErrorCodes.LedgerAccount.CannotDeleteWithAccountingHistory);

        await using var transaction = await _transactionManager.BeginTransactionAsync(cancellationToken);
        try
        {
            ledgerAccount.SoftDelete(_currentUser.UserId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Phase 2: now the vacated slot is genuinely free at the DB level —
            // safe to shift survivors into it regardless of statement order.
            await _reorderService.CloseGapAsync(ledgerAccount, cancellationToken,
                p => p.TenantId == ledgerAccount.TenantId && p.ParentId == ledgerAccount.ParentId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
