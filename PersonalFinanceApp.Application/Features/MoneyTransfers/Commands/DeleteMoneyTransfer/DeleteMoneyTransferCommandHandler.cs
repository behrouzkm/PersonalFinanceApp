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

namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Commands.DeleteMoneyTransfer;

public class DeleteMoneyTransferCommandHandler : IRequestHandler<DeleteMoneyTransferCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILedgerBalanceValidationService _ledgerValidator;
    private readonly IAccountingLookupService _lookupService;
    private readonly IAttachmentService _attachmentService;
    private readonly IUnitOfWork _unitOfWork;


    public DeleteMoneyTransferCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ILedgerBalanceValidationService ledgerValidator,
        IAccountingLookupService lookupService,
        IAttachmentService attachmentService,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _currentUser = currentUser;
        _ledgerValidator = ledgerValidator;
        _lookupService = lookupService;
        _attachmentService = attachmentService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteMoneyTransferCommand request, CancellationToken cancellationToken)
    {
        var transferDocument = await _context.AccountingDocuments
            .Include(d => d.Entries)
            .FirstOrDefaultAsync(d => d.Id == request.MoneyTransferDocumentId, cancellationToken)
            ?? throw new NotFoundException(nameof(AccountingDocument), request.MoneyTransferDocumentId);

        // row version check for concurrency control
        _context.Entry(transferDocument).Property(d => d.RowVersion).OriginalValue = request.RowVersion;

        // load every money account that could be touched
        var existingCreditEntry = transferDocument.Entries.FirstOrDefault(r => r.Credit > 0)
                ?? throw new BusinessRuleException(ApplicationErrorCodes.MoneyTransfer.CreditEntryNotFound);

        var existingDebitEntry = transferDocument.Entries.FirstOrDefault(r => r.Debit > 0)
                ?? throw new BusinessRuleException(ApplicationErrorCodes.MoneyTransfer.DebitEntryNotFound);


        var (fromFundSource, _) = await _lookupService.GetFundSourceByLedgerAccountIdAsync(existingCreditEntry.LedgerAccountId, cancellationToken);
        var (toFundSource, _) = await _lookupService.GetFundSourceByLedgerAccountIdAsync(existingDebitEntry.LedgerAccountId, cancellationToken);


        if (fromFundSource == null)
            throw new BusinessRuleException(ApplicationErrorCodes.MoneyTransfer.FromFundSourceNotFound);

        if (toFundSource == null)
            throw new BusinessRuleException(ApplicationErrorCodes.MoneyTransfer.ToFundSourceNotFound);

        await _ledgerValidator.ValidateRemovalAsync(toFundSource, existingDebitEntry.Id, cancellationToken);

        // reverse the debit/credit entries
        fromFundSource.AdjustBalance(existingCreditEntry.Credit);
        toFundSource.AdjustBalance(-existingDebitEntry.Debit);


        // soft delete the document and its entries
        transferDocument.SoftDelete(_currentUser.UserId);

        await _attachmentService.SoftDeleteAllForOwnerAsync(
            AttachmentOwnerType.AccountingDocument, transferDocument.Id, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

    }
}
