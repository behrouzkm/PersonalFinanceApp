using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Features.LedgerAccounts.Common;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Queries.GetLedgerAccountById;

public class GetLedgerAccountByIdQueryHandler : IRequestHandler<GetLedgerAccountByIdQuery, LedgerAccountDto>
{
    private readonly IApplicationDbContext _context;

    public GetLedgerAccountByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LedgerAccountDto> Handle(GetLedgerAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var ledgerAccount = await _context.LedgerAccounts
                 .FirstOrDefaultAsync(d => d.Id == request.LedgerAccountId, cancellationToken)
            ?? throw new NotFoundException(nameof(LedgerAccount), request.LedgerAccountId);

        return new LedgerAccountDto
        {
            Id = ledgerAccount.Id,
            AccountTypeId = ledgerAccount.AccountTypeId,
            IsPostingAccount = ledgerAccount.IsPostingAccount,
            HasBeenUsedInEntries = ledgerAccount.HasBeenUsedInEntries,
            Name = ledgerAccount.Name,
            ParentId = ledgerAccount.ParentId
        };

    }
}
