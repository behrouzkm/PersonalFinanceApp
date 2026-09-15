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

namespace PersonalFinanceApp.Application.Features.CashAccounts.Commands.UpdateCashAccount;

public class UpdateCashAccountCommandHandler : IRequestHandler<UpdateCashAccountCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IOpeningBalanceService _openingBalanceService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCashAccountCommandHandler(
                IApplicationDbContext context,
                ICurrentUserService currentUser,
                IOpeningBalanceService openingBalanceService,
                IUnitOfWork unitOfWork)
    {
        _context = context;
        _currentUser = currentUser;
        _openingBalanceService = openingBalanceService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateCashAccountCommand request, CancellationToken cancellationToken)
    {

        var cashAccount = await _context.CashAccounts
        .FirstOrDefaultAsync(r => r.Id == request.CashAccountId, cancellationToken)
        ?? throw new NotFoundException(nameof(CashAccount), request.CashAccountId);

        _context.Entry(cashAccount).Property(d => d.RowVersion).OriginalValue = request.RowVersion;

        var oldInitialBalance = cashAccount.InitialBalance;
        var oldCurrencyId = cashAccount.CurrencyId;
        var existingOpeningDocId = cashAccount.OpeningAccountingDocumentId;

        // Mutate first — ReconcileAsync/ValidateAsync read state off this reference,
        // so it must already reflect the requested values before we call it.
        cashAccount.UpdateCashAccount(
            request.DisplayName,
            request.CurrencyId,
            request.OpeningDate,
            request.InitialBalance,
            _currentUser.UserId,
            request.Location,
            request.IsPhysical,
            request.Description);

        var openingDocId = await _openingBalanceService.ReconcileAsync(
            cashAccount,
            existingOpeningDocId,
            oldInitialBalance,
            0,
            oldCurrencyId,
            AccountCategory.CashAccount,
            request.Description,
            cancellationToken);

        cashAccount.UpdateOpeningAccountingDocumentId(openingDocId, _currentUser.UserId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
