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

namespace PersonalFinanceApp.Application.Features.CashAccounts.Commands.DeleteCashAccount;

public class DeleteCashAccountCommandHandler : IRequestHandler<DeleteCashAccountCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IReorderService _reorderService;
    private readonly IAttachmentService _attachmentService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransactionManager _transactionManager;

    public DeleteCashAccountCommandHandler(
                IApplicationDbContext context,
                ICurrentUserService currentUser,
                IReorderService reorderService,
                IAttachmentService attachmentService,
                IUnitOfWork unitOfWork,
                ITransactionManager transactionManager)
    {
        _context = context;
        _currentUser = currentUser;
        _reorderService = reorderService;
        _attachmentService = attachmentService;
        _unitOfWork = unitOfWork;
        _transactionManager =transactionManager;
    }

    public async Task Handle(DeleteCashAccountCommand request, CancellationToken cancellationToken)
    {

        var cashAccount = await _context.CashAccounts
        .Include(r => r.LedgerAccount)
        .Include(r => r.OpeningAccountingDocument)
            .ThenInclude(d => d!.Entries)
        .FirstOrDefaultAsync(r => r.Id == request.CashAccountId, cancellationToken)
        ?? throw new NotFoundException(nameof(CashAccount), request.CashAccountId);

        _context.Entry(cashAccount).Property(d => d.RowVersion).OriginalValue = request.RowVersion;

        var hasAccountingHistory = await _context.AccountingEntries
            .AnyAsync(r => r.AccountingDocumentId != cashAccount.OpeningAccountingDocumentId &&
                 r.LedgerAccountId == cashAccount.LedgerAccountId, cancellationToken);

        if (hasAccountingHistory)
            throw new BusinessRuleException(ApplicationErrorCodes.CashAccount.CannotDeleteWithAccountingHistory);

        await using var transaction = await _transactionManager.BeginTransactionAsync(cancellationToken);
        try
        {
            // Phase 1: commit the deletion first — this is what actually vacates
            // the slot under the filtered unique index.
            if (cashAccount.OpeningAccountingDocumentId is not null)
                cashAccount.OpeningAccountingDocument!.SoftDelete(_currentUser.UserId);

            cashAccount.LedgerAccount.SoftDelete(_currentUser.UserId);
            cashAccount.SoftDelete(_currentUser.UserId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Phase 2: now the vacated slot is genuinely free at the DB level —
            // safe to shift survivors into it regardless of statement order.
            await _reorderService.CloseGapAsync(cashAccount, cancellationToken, p => p.TenantId == cashAccount.TenantId);
            await _reorderService.CloseGapAsync(cashAccount.LedgerAccount, cancellationToken,
                p => p.TenantId == cashAccount.TenantId && p.ParentId == cashAccount.LedgerAccount.ParentId);


            await _attachmentService.SoftDeleteAllForOwnerAsync(
                AttachmentOwnerType.MonetaryAccount, cashAccount.Id, cancellationToken);

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
