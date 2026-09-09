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

namespace PersonalFinanceApp.Application.Features.CashAccounts.Commands.CreateCashAccount;

public class CreateCashAccountCommandHandler : IRequestHandler<CreateCashAccountCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IOpeningBalanceService _openingBalanceService;
    public CreateCashAccountCommandHandler(
                IApplicationDbContext context,
                ICurrentUserService currentUser,
                IOpeningBalanceService openingBalanceService)
    {
        _context = context;
        _currentUser = currentUser;
        _openingBalanceService = openingBalanceService;
    }

    public async Task<Guid> Handle(CreateCashAccountCommand request, CancellationToken cancellationToken)
    {
        var (ledgerAccount, openingDocId) = await _openingBalanceService.CreateAsync(
            request.ParentLedgerId, AccountCategory.CashAccount,
            request.DisplayName, request.OpeningDate, request.CurrencyId,
            request.InitialBalance, request.CreditLimit, request.Description, cancellationToken);

        var maxDisplayOrder = await _context.CashAccounts.MaxAsync(c => (int?)c.DisplayOrder, cancellationToken) ?? 0;

        var cashAccount = new CashAccount(
            request.DisplayName,
            ledgerAccount.Id,
            request.CurrencyId,
            request.OpeningDate,
            request.InitialBalance,
            maxDisplayOrder + 1,
            request.Location,
            _currentUser.TenantId,
            _currentUser.UserId,
            request.IsPhysical,
            request.Description,
            openingDocId
        );

        await _context.CashAccounts.AddAsync(cashAccount, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return cashAccount.Id;
    }
}
