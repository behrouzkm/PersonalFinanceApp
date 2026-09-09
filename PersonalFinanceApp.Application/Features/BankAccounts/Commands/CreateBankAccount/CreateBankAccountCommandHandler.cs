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

namespace PersonalFinanceApp.Application.Features.BankAccounts.Commands.CreateBankAccount;

public class CreateBankAccountCommandHandler : IRequestHandler<CreateBankAccountCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IOpeningBalanceService _openingBalanceService;
    public CreateBankAccountCommandHandler(
                IApplicationDbContext context,
                ICurrentUserService currentUser,
                IOpeningBalanceService openingBalanceService)
    {
        _context = context;
        _currentUser = currentUser;
        _openingBalanceService = openingBalanceService;
    }

    public async Task<Guid> Handle(CreateBankAccountCommand request, CancellationToken cancellationToken)
    {
        var (ledgerAccount, openingDocId) = await _openingBalanceService.CreateAsync(
            request.ParentLedgerId, AccountCategory.BankAccount,
            request.DisplayName, request.OpeningDate, request.CurrencyId,
            request.InitialBalance, request.CreditLimit, request.Description, cancellationToken);

        var maxDisplayOrder = await _context.BankAccounts.MaxAsync(c => (int?)c.DisplayOrder, cancellationToken) ?? 0;

        var bankAccount = new BankAccount(
            request.DisplayName,
            ledgerAccount.Id,
            request.CurrencyId,
            request.OpeningDate,
            request.InitialBalance,
            maxDisplayOrder + 1,
            _currentUser.TenantId,
            _currentUser.UserId,
            request.BankName,
            request.BranchName,
            request.BankAccountType,
            request.BankAccountNumber,
            request.IBAN,
            request.Description,
            request.CreditLimit,
            openingDocId
        );

        await _context.BankAccounts.AddAsync(bankAccount, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return bankAccount.Id;
    }
}
