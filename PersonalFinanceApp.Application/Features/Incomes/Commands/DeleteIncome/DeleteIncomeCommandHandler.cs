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

namespace PersonalFinanceApp.Application.Features.Incomes.Commands.DeleteIncome;

public class DeleteIncomeCommandHandler : IRequestHandler<DeleteIncomeCommand>
{
    public readonly IApplicationDbContext _context;
    public readonly ICurrentUserService _currentUser;
    private readonly IAttachmentService _attachmentService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteIncomeCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IAttachmentService attachmentService,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _currentUser = currentUserService;
        _attachmentService = attachmentService;
        _unitOfWork=unitOfWork;
    }

    public async Task Handle(DeleteIncomeCommand request, CancellationToken cancellationToken)
    {
        var document = await _context.AccountingDocuments
               .Include(d => d.Entries)
               .FirstOrDefaultAsync(d => d.Id == request.AccountingDocumentId, cancellationToken)
               ?? throw new NotFoundException(nameof(AccountingDocument), request.AccountingDocumentId);

        // row version check for concurrency control
        _context.Entry(document).Property(d => d.RowVersion).OriginalValue = request.RowVersion;

        var debitEntries = document.Entries.Where(e => e.Debit > 0).ToList();
        var debitEntriesLedgerAccountIds = debitEntries.Select(e => e.LedgerAccountId).ToHashSet();

        var monetaryAccounts = await _context.MonetaryAccounts
            .Where(ma => debitEntriesLedgerAccountIds.Contains(ma.LedgerAccountId))
            .ToDictionaryAsync(ma => ma.LedgerAccountId, cancellationToken);

        // reverse the debit entries
        foreach (var entry in debitEntries)
        {
            if (monetaryAccounts.TryGetValue(entry.LedgerAccountId, out var monetaryAccount))
            {
                monetaryAccount.AdjustBalance(-entry.Debit);
            }
        }

        // soft delete the document and its entries
        document.SoftDelete(_currentUser.UserId);

        await _attachmentService.SoftDeleteAllForOwnerAsync(
           AttachmentOwnerType.AccountingDocument, document.Id, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
