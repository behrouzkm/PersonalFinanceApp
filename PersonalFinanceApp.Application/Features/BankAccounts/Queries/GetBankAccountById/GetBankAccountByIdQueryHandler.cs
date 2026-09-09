using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Features.BankAccounts.Common;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.BankAccounts.Queries.GetBankAccountById;

public class GetBankAccountByIdQueryHandler : IRequestHandler<GetBankAccountByIdQuery, BankAccountDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAttachmentService _attachmentService;

    public GetBankAccountByIdQueryHandler(IApplicationDbContext context, IAttachmentService attachmentService)
    {
        _context = context;
        _attachmentService = attachmentService;
    }

    public async Task<BankAccountDto> Handle(GetBankAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var bankAccount = await _context.BankAccounts
            .Include(p => p.Currency)
                .FirstOrDefaultAsync(d => d.Id == request.BankAccountId, cancellationToken)
            ?? throw new NotFoundException(nameof(BankAccount), request.BankAccountId);

        var attachments = await _attachmentService.GetForOwnerAsync(
            AttachmentOwnerType.MonetaryAccount, bankAccount.Id, cancellationToken);

        return new BankAccountDto
        {
            BankId = bankAccount.Id,
            BankAccountType = bankAccount.BankAccountType,
            DisplayName = bankAccount.DisplayName,
            LedgerAccountId = bankAccount.LedgerAccountId,
            OpeningDate = bankAccount.OpeningDate,
            InitialBalance = bankAccount.InitialBalance,
            CurrentBalance = bankAccount.CurrentBalance,
            CreditLimit = bankAccount.CreditLimit,
            OpeningAccountingDocumentId = bankAccount.OpeningAccountingDocumentId,
            CurrencyId = bankAccount.CurrencyId,
            CurrencyName = bankAccount.Currency.Name,
            CurrencySymbol = bankAccount.Currency.Symbol,
            DisplayOrder = bankAccount.DisplayOrder,
            BankName = bankAccount.BankName,
            BranchName = bankAccount.BranchName,
            BankAccountNumber = bankAccount.BankAccountNumber,
            IBAN = bankAccount.IBAN,
            Attachments = attachments
        };

    }
}
