using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.BankAccounts.Common;


namespace PersonalFinanceApp.Application.Features.BankAccounts.Queries.GetBankAccountsList;

public class GetBankAccountsListQueryHandler : IRequestHandler<GetBankAccountsListQuery, PaginatedList<BankAccountListItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBankAccountsListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<BankAccountListItemDto>> Handle(GetBankAccountsListQuery request,
                        CancellationToken cancellationToken)
    {
        var query = _context.BankAccounts.Include(r => r.Currency).AsQueryable();

        if (request.BankAccountType.HasValue)
            query = query.Where(r => r.BankAccountType == request.BankAccountType.Value);

        if (request.CurrencyId.HasValue)
            query = query.Where(r => r.CurrencyId == request.CurrencyId.Value);

        var projection = query
            .OrderBy(o => o.DisplayOrder)
            .Select(r => new BankAccountListItemDto
            {
                BankId = r.Id,
                BankAccountType = r.BankAccountType,
                DisplayName = r.DisplayName,
                LedgerAccountId = r.LedgerAccountId,
                OpeningDate = r.OpeningDate,
                InitialBalance = r.InitialBalance,
                CurrentBalance = r.CurrentBalance,
                CreditLimit = r.CreditLimit,
                OpeningAccountingDocumentId = r.OpeningAccountingDocumentId,
                CurrencyId = r.CurrencyId,
                CurrencyName = r.Currency.Name,
                CurrencySymbol = r.Currency.Symbol,
                DisplayOrder = r.DisplayOrder,
                BankName = r.BankName,
                BranchName = r.BranchName,
                BankAccountNumber = r.BankAccountNumber,
                IBAN = r.IBAN,
                AttachmentCount=_context.Attachments.Count(r=>r.MonetaryAccountId == r.Id)
            });

        return await PaginatedList<BankAccountListItemDto>.CreateAsync(projection, request.PageNumber,
                       request.PageSize, cancellationToken);
    }
}
