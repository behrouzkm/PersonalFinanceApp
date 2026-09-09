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
    private readonly ILedgerBalanceValidationService _ledgerValidator;
    public UpdateMoneyTransferCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ILedgerBalanceValidationService ledgerValidator)
    {
        _context = context;
        _currentUser = currentUser;
        _ledgerValidator = ledgerValidator;
    }

    public async Task Handle(UpdateMoneyTransferCommand request, CancellationToken cancellationToken)
    {
        // load the document to update, including all its entries
        var transferDocument = await _context.AccountingDocuments
            .Include(d => d.Entries)
            .FirstOrDefaultAsync(r => r.Id == request.MoneyTransferDocumentId, cancellationToken)
            ?? throw new NotFoundException(nameof(AccountingDocument), request.MoneyTransferDocumentId);

        // load and validate ToMonetaryAccount (debit side)
        var toMonetaryAccount = await _context.MonetaryAccounts
                .Include(m => m.LedgerAccount)
                .FirstOrDefaultAsync(r => r.Id == request.ToMonetaryAccountId, cancellationToken)
            ?? throw new NotFoundException(nameof(MonetaryAccount), request.ToMonetaryAccountId);

        // load and validate FromMonetaryAccount (credit side)
        var fromMonetaryAccount = await _context.MonetaryAccounts
                .Include(m => m.LedgerAccount)
                .FirstOrDefaultAsync(r => r.Id == request.FromMonetaryAccountId, cancellationToken)
            ?? throw new NotFoundException(nameof(MonetaryAccount), request.FromMonetaryAccountId);


        _context.Entry(transferDocument).Property(d => d.RowVersion).OriginalValue = request.RowVersion;

        // update the document's header fields
        transferDocument.UpdateAccountingDocument(request.TransferDate, request.CurrencyId, _currentUser.UserId, request.Description);

        transferDocument.EnsureCurrencyMatches(fromMonetaryAccount.CurrencyId);
        transferDocument.EnsureCurrencyMatches(toMonetaryAccount.CurrencyId);




        // load every money account that could be touched
        var existingCreditEntry = transferDocument.Entries.FirstOrDefault(r => r.Credit > 0)
                ?? throw new BusinessRuleException(ApplicationErrorCodes.MoneyTransfer.CreditEntryNotFound);

        var existingDebitEntry = transferDocument.Entries.FirstOrDefault(r => r.Debit > 0)
                ?? throw new BusinessRuleException(ApplicationErrorCodes.MoneyTransfer.DebitEntryNotFound);


        // --- Credit side (From) ---
        if (existingCreditEntry.LedgerAccountId != fromMonetaryAccount.LedgerAccountId)
        {
            if (!fromMonetaryAccount.CanWithdraw(request.Amount))
                throw new BusinessRuleException(ApplicationErrorCodes.MoneyTransfer.InsufficientBalance,
                                                    fromMonetaryAccount.Id, request.Amount);

            await _ledgerValidator.ValidateAsync(fromMonetaryAccount, request.TransferDate, 0, request.Amount,
                replacingEntryId: null, cancellationToken);

            var oldFromMonetaryAccount = await _context.MonetaryAccounts
                    .FirstOrDefaultAsync(r => r.LedgerAccountId == existingCreditEntry.LedgerAccountId, cancellationToken)
                    ?? throw new BusinessRuleException(ApplicationErrorCodes.MoneyTransfer.FromMonetaryAccountIdRequired);

            oldFromMonetaryAccount.AdjustBalance(existingCreditEntry.Credit);

            existingCreditEntry.UpdateEntry(fromMonetaryAccount.LedgerAccountId, 0, request.Amount, request.Description);
            existingCreditEntry.UpdateAudit(_currentUser.UserId);

            fromMonetaryAccount.LedgerAccount.MarkAsUsed();
            fromMonetaryAccount.AdjustBalance(-request.Amount);
        }
        else if (request.Amount != existingCreditEntry.Credit)
        {
            var amountDelta = request.Amount - existingCreditEntry.Credit;

            if (amountDelta > 0)
            {
                if (!fromMonetaryAccount.CanWithdraw(amountDelta))
                    throw new BusinessRuleException(ApplicationErrorCodes.MoneyTransfer.InsufficientBalance,
                                                        fromMonetaryAccount.Id, amountDelta);

            }

            existingCreditEntry.SetAmounts(0, request.Amount);
            existingCreditEntry.SetDescription(request.Description);
            existingCreditEntry.UpdateAudit(_currentUser.UserId);

            fromMonetaryAccount.AdjustBalance(-amountDelta);
        }
        else
        {
            existingCreditEntry.SetDescription(request.Description);
            existingCreditEntry.UpdateAudit(_currentUser.UserId);
        }

        // --- Debit side (To) ---
        if (existingDebitEntry.LedgerAccountId != toMonetaryAccount.LedgerAccountId)
        {
            var oldToMonetaryAccount = await _context.MonetaryAccounts
                  .FirstOrDefaultAsync(r => r.LedgerAccountId == existingDebitEntry.LedgerAccountId, cancellationToken)
                  ?? throw new BusinessRuleException(ApplicationErrorCodes.MoneyTransfer.FromMonetaryAccountIdRequired);

            await _ledgerValidator.ValidateRemovalAsync(oldToMonetaryAccount, existingDebitEntry.Id, cancellationToken);


            oldToMonetaryAccount.AdjustBalance(-existingDebitEntry.Debit);

            existingDebitEntry.UpdateEntry(toMonetaryAccount.LedgerAccountId, request.Amount, 0, request.Description);
            existingDebitEntry.UpdateAudit(_currentUser.UserId);

            toMonetaryAccount.LedgerAccount.MarkAsUsed();
            toMonetaryAccount.AdjustBalance(request.Amount);
        }
        else if (request.Amount != existingDebitEntry.Debit)
        {
            var amountDelta = request.Amount - existingDebitEntry.Debit;

            if (amountDelta < 0)
                await _ledgerValidator.ValidateAsync(toMonetaryAccount, request.TransferDate, request.Amount, 0,
                      replacingEntryId: existingDebitEntry.Id, cancellationToken);


            existingDebitEntry.SetAmounts(request.Amount, 0);
            existingDebitEntry.SetDescription(request.Description);
            existingDebitEntry.UpdateAudit(_currentUser.UserId);

            toMonetaryAccount.AdjustBalance(amountDelta);
        }
        else
        {
            existingDebitEntry.SetDescription(request.Description);
            existingDebitEntry.UpdateAudit(_currentUser.UserId);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

}
