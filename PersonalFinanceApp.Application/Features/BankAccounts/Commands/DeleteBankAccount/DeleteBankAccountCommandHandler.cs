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

namespace PersonalFinanceApp.Application.Features.BankAccounts.Commands.DeleteBankAccount;

public class DeleteBankAccountCommandHandler : IRequestHandler<DeleteBankAccountCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IReorderService _reorderService;
    private readonly IAttachmentService _attachmentService;

    public DeleteBankAccountCommandHandler(
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

    public async Task Handle(DeleteBankAccountCommand request, CancellationToken cancellationToken)
    {

        var bankAccount = await _context.BankAccounts
        .Include(r => r.LedgerAccount)
        .Include(r => r.OpeningAccountingDocument)
            .ThenInclude(d => d!.Entries)
        .FirstOrDefaultAsync(r => r.Id == request.BankAccountId, cancellationToken)
        ?? throw new NotFoundException(nameof(BankAccount), request.BankAccountId);

        _context.Entry(bankAccount).Property(d => d.RowVersion).OriginalValue = request.RowVersion;

        var hasAccountingHistory = await _context.AccountingEntries
            .AnyAsync(r => r.AccountingDocumentId != bankAccount.OpeningAccountingDocumentId &&
                 r.LedgerAccountId == bankAccount.LedgerAccountId, cancellationToken);

        if (hasAccountingHistory)
            throw new BusinessRuleException(ApplicationErrorCodes.BankAccount.CannotDeleteWithAccountingHistory);

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Phase 1: commit the deletion first — this is what actually vacates
            // the slot under the filtered unique index.
            if (bankAccount.OpeningAccountingDocumentId is not null)
                bankAccount.OpeningAccountingDocument!.SoftDelete(_currentUser.UserId);

            bankAccount.LedgerAccount.SoftDelete(_currentUser.UserId);
            bankAccount.SoftDelete(_currentUser.UserId);
            await _context.SaveChangesAsync(cancellationToken);

            // Phase 2: now the vacated slot is genuinely free at the DB level —
            // safe to shift survivors into it regardless of statement order.
            await _reorderService.CloseGapAsync(bankAccount, cancellationToken, p => p.TenantId == bankAccount.TenantId);
            await _reorderService.CloseGapAsync(bankAccount.LedgerAccount, cancellationToken,
                p => p.TenantId == bankAccount.TenantId && p.ParentId == bankAccount.LedgerAccount.ParentId);


            await _attachmentService.SoftDeleteAllForOwnerAsync(
                AttachmentOwnerType.MonetaryAccount, bankAccount.Id, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);


            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
