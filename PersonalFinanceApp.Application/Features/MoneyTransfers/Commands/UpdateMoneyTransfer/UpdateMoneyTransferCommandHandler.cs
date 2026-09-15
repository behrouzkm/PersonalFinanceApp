using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Commands.UpdateMoneyTransfer;

public class UpdateMoneyTransferCommandHandler : IRequestHandler<UpdateMoneyTransferCommand>
{

    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAccountingLookupService _lookupService;
    private readonly ILedgerBalanceValidationService _ledgerValidator;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMoneyTransferCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IAccountingLookupService lookupService,
        ILedgerBalanceValidationService ledgerValidator,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _currentUser = currentUser;
        _lookupService = lookupService;
        _ledgerValidator = ledgerValidator;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateMoneyTransferCommand request, CancellationToken cancellationToken)
    {
        // load the document to update, including all its entries
        var transferDocument = await _context.AccountingDocuments
            .Include(d => d.Entries)
            .FirstOrDefaultAsync(r => r.Id == request.MoneyTransferDocumentId, cancellationToken)
            ?? throw new NotFoundException(nameof(AccountingDocument), request.MoneyTransferDocumentId);



        var (newFromFundSource, newFromLedgerAccount) = await _lookupService
                  .GetFundSourceByLedgerAccountIdAsync(request.FromLedgerAccountId, cancellationToken);
        var (newToFundSource, newToLedgerAccount) = await _lookupService
            .GetFundSourceByLedgerAccountIdAsync(request.ToLedgerAccountId, cancellationToken);

        if (newFromFundSource.CurrencyId != newToFundSource.CurrencyId)
            throw new BusinessRuleException(ApplicationErrorCodes.MoneyTransfer.SourceDestinationCurrencyMismatch);



        _context.Entry(transferDocument).Property(d => d.RowVersion).OriginalValue = request.RowVersion;

        // update the document's header fields
        transferDocument.UpdateAccountingDocument(request.TransferDate, request.CurrencyId, _currentUser.UserId, request.Description);

        transferDocument.EnsureCurrencyMatches(newFromFundSource.CurrencyId);
        transferDocument.EnsureCurrencyMatches(newToFundSource.CurrencyId);




        // load every money account that could be touched
        var existingCreditEntry = transferDocument.Entries.FirstOrDefault(r => r.Credit > 0)
                ?? throw new BusinessRuleException(ApplicationErrorCodes.MoneyTransfer.CreditEntryNotFound);

        var existingDebitEntry = transferDocument.Entries.FirstOrDefault(r => r.Debit > 0)
                ?? throw new BusinessRuleException(ApplicationErrorCodes.MoneyTransfer.DebitEntryNotFound);


        // --- Credit side (From) ---
        if (existingCreditEntry.LedgerAccountId != newFromFundSource.LedgerAccountId)
        {
            if (!newFromFundSource.CanWithdraw(request.Amount))
                throw new BusinessRuleException(ApplicationErrorCodes.MoneyTransfer.InsufficientBalance,
                                                    newFromFundSource.Id, request.Amount);

            await _ledgerValidator.ValidateAsync(newFromFundSource, request.TransferDate, 0, request.Amount,
                replacingEntryId: null, cancellationToken);

            var (oldFromFundSource, _) = await _lookupService.GetFundSourceByLedgerAccountIdAsync(existingCreditEntry.LedgerAccountId, cancellationToken);

            if (oldFromFundSource == null)
                throw new BusinessRuleException(ApplicationErrorCodes.MoneyTransfer.FromFundSourceNotFound);

            oldFromFundSource.AdjustBalance(existingCreditEntry.Credit);

            existingCreditEntry.UpdateEntry(newFromFundSource.LedgerAccountId, 0, request.Amount, _currentUser.UserId, request.Description);

            newFromLedgerAccount.MarkAsUsed();
            newFromFundSource.AdjustBalance(-request.Amount);
        }
        else if (request.Amount != existingCreditEntry.Credit)
        {
            var amountDelta = request.Amount - existingCreditEntry.Credit;

            if (amountDelta > 0)
            {
                if (!newFromFundSource.CanWithdraw(amountDelta))
                    throw new BusinessRuleException(ApplicationErrorCodes.MoneyTransfer.InsufficientBalance,
                                                        newFromFundSource.Id, amountDelta);

            }

            existingCreditEntry.UpdateEntry(0, request.Amount, _currentUser.UserId, request.Description);

            newFromFundSource.AdjustBalance(-amountDelta);
        }
        else
        {
            existingCreditEntry.SetDescription(request.Description);
            existingCreditEntry.UpdateAudit(_currentUser.UserId);
        }

        // --- Debit side (To) ---
        if (existingDebitEntry.LedgerAccountId != newToFundSource.LedgerAccountId)
        {

            await _ledgerValidator.ValidateAsync(newFromFundSource, request.TransferDate, 0, request.Amount,
                replacingEntryId: null, cancellationToken);

            var (oldToFundSource, _) = await _lookupService.GetFundSourceByLedgerAccountIdAsync(existingCreditEntry.LedgerAccountId, cancellationToken);

            if (oldToFundSource == null)
                throw new BusinessRuleException(ApplicationErrorCodes.MoneyTransfer.ToFundSourceNotFound);

            await _ledgerValidator.ValidateRemovalAsync(oldToFundSource, existingDebitEntry.Id, cancellationToken);


            oldToFundSource.AdjustBalance(-existingDebitEntry.Debit);

            existingDebitEntry.UpdateEntry(newToFundSource.LedgerAccountId, request.Amount, 0, _currentUser.UserId, request.Description);

            newToLedgerAccount.MarkAsUsed();
            newToFundSource.AdjustBalance(request.Amount);
        }
        else if (request.Amount != existingDebitEntry.Debit)
        {
            var amountDelta = request.Amount - existingDebitEntry.Debit;

            if (amountDelta < 0)
                await _ledgerValidator.ValidateAsync(newToFundSource, request.TransferDate, request.Amount, 0,
                      replacingEntryId: existingDebitEntry.Id, cancellationToken);


            existingDebitEntry.UpdateEntry(request.Amount, 0, _currentUser.UserId, request.Description);

            newToFundSource.AdjustBalance(amountDelta);
        }
        else
        {
            existingDebitEntry.SetDescription(request.Description);
            existingDebitEntry.UpdateAudit(_currentUser.UserId);
        }

        transferDocument.EnsureBalanced();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

}
