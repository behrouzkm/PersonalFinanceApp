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

namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Commands.RestoreMoneyTransfer;

public class RestoreMoneyTransferCommandHandler : IRequestHandler<RestoreMoneyTransferCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILedgerBalanceValidationService _ledgerValidator;
    private readonly IAccountingLookupService _lookupService;
    private readonly IAttachmentService _attachmentService;
    private readonly IUnitOfWork _unitOfWork;

    public RestoreMoneyTransferCommandHandler(
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
    public async Task Handle(RestoreMoneyTransferCommand request, CancellationToken cancellationToken)
    {
        var transferDocument = await _context.AccountingDocuments
            .IgnoreQueryFilters()
            .Include(d => d.Entries)
            .FirstOrDefaultAsync(d => d.Id == request.MoneyTransferDocumentId
                && d.DocumentType == Domain.Enums.DocumentType.MoneyTransfer
                && d.TenantId == _currentUser.TenantId, cancellationToken)
            ?? throw new NotFoundException(nameof(AccountingDocument), request.MoneyTransferDocumentId);

        transferDocument.Restore(_currentUser.UserId);


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


        await _ledgerValidator.ValidateAsync(fromFundSource, transferDocument.DocumentDate, 0, existingCreditEntry.Credit,
                           replacingEntryId: existingCreditEntry.Id, cancellationToken);

        fromFundSource.AdjustBalance(-existingCreditEntry.Credit);
        toFundSource.AdjustBalance(existingDebitEntry.Debit);

        await _attachmentService.RestoreAllForOwnerAsync(
            AttachmentOwnerType.AccountingDocument, transferDocument.Id, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
