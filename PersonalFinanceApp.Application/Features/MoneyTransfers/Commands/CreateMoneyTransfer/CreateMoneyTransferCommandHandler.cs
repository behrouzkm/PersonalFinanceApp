using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Commands.CreateMoneyTransfer;

public class CreateMoneyTransferCommandHandler : IRequestHandler<CreateMoneyTransferCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILedgerBalanceValidationService _ledgerValidator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMoneyTransferCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ILedgerBalanceValidationService ledgerValidator,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _currentUser = currentUser;
        _ledgerValidator = ledgerValidator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateMoneyTransferCommand request, CancellationToken cancellationToken)
    {
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


        var accountingDocument = new AccountingDocument(DocumentType.MoneyTransfer, request.TransferDate,
                request.CurrencyId, _currentUser.TenantId, _currentUser.UserId, request.Description);

        accountingDocument.EnsureCurrencyMatches(fromMonetaryAccount.CurrencyId);
        accountingDocument.EnsureCurrencyMatches(toMonetaryAccount.CurrencyId);

        if (!fromMonetaryAccount.CanWithdraw(request.Amount))
            throw new BusinessRuleException(ApplicationErrorCodes.MoneyTransfer.InsufficientBalance,
                                                fromMonetaryAccount.Id, request.Amount);

        await _ledgerValidator.ValidateAsync(fromMonetaryAccount, request.TransferDate, 0, request.Amount, replacingEntryId: null, cancellationToken);

        accountingDocument.AddEntry(toMonetaryAccount.LedgerAccountId, request.Amount, 0, request.Description, _currentUser.UserId);
        accountingDocument.AddEntry(fromMonetaryAccount.LedgerAccountId, 0, request.Amount, request.Description, _currentUser.UserId);

        fromMonetaryAccount.LedgerAccount.MarkAsUsed();
        toMonetaryAccount.LedgerAccount.MarkAsUsed();

        fromMonetaryAccount.AdjustBalance(-request.Amount);
        toMonetaryAccount.AdjustBalance(request.Amount);

        accountingDocument.EnsureBalanced();

        _context.AccountingDocuments.Add(accountingDocument);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return accountingDocument.Id;
    }
}
