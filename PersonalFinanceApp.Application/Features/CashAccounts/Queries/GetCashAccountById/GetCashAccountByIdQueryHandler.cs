using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Features.CashAccounts.Common;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.CashAccounts.Queries.GetCashAccountById;

public class GetCashAccountByIdQueryHandler : IRequestHandler<GetCashAccountByIdQuery, CashAccountDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAttachmentService _attachmentService;

    public GetCashAccountByIdQueryHandler(IApplicationDbContext context, IAttachmentService attachmentService)
    {
        _context = context;
        _attachmentService = attachmentService;
    }

    public async Task<CashAccountDto> Handle(GetCashAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var cashAccount = await _context.CashAccounts
            .Include(p => p.Currency)
                .FirstOrDefaultAsync(d => d.Id == request.CashAccountId, cancellationToken)
            ?? throw new NotFoundException(nameof(CashAccount), request.CashAccountId);

        var attachments = await _attachmentService.GetForOwnerAsync(
            AttachmentOwnerType.AccountingDocument, cashAccount.Id, cancellationToken);

        return new CashAccountDto
        {
            Id = cashAccount.Id,
            DisplayName = cashAccount.DisplayName,
            LedgerAccountId = cashAccount.LedgerAccountId,
            OpeningDate = cashAccount.OpeningDate,
            InitialBalance = cashAccount.InitialBalance,
            CurrentBalance = cashAccount.CurrentBalance,
            OpeningAccountingDocumentId = cashAccount.OpeningAccountingDocumentId,
            CurrencyId = cashAccount.CurrencyId,
            CurrencyName = cashAccount.Currency.Name,
            CurrencySymbol = cashAccount.Currency.Symbol,
            DisplayOrder = cashAccount.DisplayOrder,
            Location = cashAccount.Location,
            IsPhysical = cashAccount.IsPhysical,
            Attachments = attachments
        };

    }
}
