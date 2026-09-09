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

namespace PersonalFinanceApp.Application.Features.BankAccounts.Commands.UpdateBankAccount;

public class UpdateBankAccountCommandHandler : IRequestHandler<UpdateBankAccountCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IOpeningBalanceService _openingBalanceService;

    public UpdateBankAccountCommandHandler(
                IApplicationDbContext context,
                ICurrentUserService currentUser,
                IOpeningBalanceService openingBalanceService)
    {
        _context = context;
        _currentUser = currentUser;
        _openingBalanceService = openingBalanceService;
    }

    public async Task Handle(UpdateBankAccountCommand request, CancellationToken cancellationToken)
    {

        var bankAccount = await _context.BankAccounts
        .FirstOrDefaultAsync(r => r.Id == request.BankAccountId, cancellationToken)
        ?? throw new NotFoundException(nameof(BankAccount), request.BankAccountId);

        _context.Entry(bankAccount).Property(d => d.RowVersion).OriginalValue = request.RowVersion;

        var oldInitialBalance = bankAccount.InitialBalance;
        var oldCreditLimit = bankAccount.CreditLimit;
        var oldCurrencyId = bankAccount.CurrencyId;
        var existingOpeningDocId = bankAccount.OpeningAccountingDocumentId;

        // Mutate first — ReconcileAsync/ValidateAsync read state off this reference,
        // so it must already reflect the requested values before we call it.
        bankAccount.UpdateBankAccount(
            request.DisplayName,
            request.CurrencyId,
            request.OpeningDate,
            request.InitialBalance,
            request.BankName,
            request.BranchName,
            request.BankAccountNumber,
            _currentUser.UserId,
            request.CreditLimit,
            request.IBAN,
            request.Description);

        var openingDocId = await _openingBalanceService.ReconcileAsync(
            bankAccount,
            existingOpeningDocId,
            oldInitialBalance,
            oldCreditLimit,
            oldCurrencyId,
            AccountCategory.BankAccount,
            request.Description,
            cancellationToken);

        bankAccount.UpdateOpeningAccountingDocumentId(openingDocId, _currentUser.UserId);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
