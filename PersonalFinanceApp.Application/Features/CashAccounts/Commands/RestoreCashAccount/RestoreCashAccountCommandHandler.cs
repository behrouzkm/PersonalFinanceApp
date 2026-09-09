using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.CashAccounts.Commands.RestoreCashAccount;

public class RestoreCashAccountCommandHandler : IRequestHandler<RestoreCashAccountCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IReorderService _reorderService;
    private readonly IAttachmentService _attachmentService;

    public RestoreCashAccountCommandHandler(
                IApplicationDbContext context,
                ICurrentUserService currentUser,
                IReorderService reorderService,
                IAttachmentService attachmentService)
    {
        _context = context;
        _currentUser = currentUser;
        _reorderService = reorderService;
        _attachmentService = attachmentService;
    }

    public async Task Handle(RestoreCashAccountCommand request, CancellationToken cancellationToken)
    {
        var cashAccount = await _context.CashAccounts
            .IgnoreQueryFilters()
            .Include(p => p.LedgerAccount)
            .Include(p => p.OpeningAccountingDocument)
                .ThenInclude(p => p!.Entries)
            .FirstOrDefaultAsync(d => d.Id == request.CashAccountId
                && d.TenantId == _currentUser.TenantId, cancellationToken)
            ?? throw new NotFoundException(nameof(CashAccount), request.CashAccountId);

        cashAccount.Restore(_currentUser.UserId);
        cashAccount.LedgerAccount.Restore(_currentUser.UserId);

        if (cashAccount.OpeningAccountingDocument is not null)
            cashAccount.OpeningAccountingDocument.Restore(_currentUser.UserId);


        await _reorderService.AppendToEndAsync(cashAccount, p => p.TenantId == cashAccount.TenantId, cancellationToken);
        await _reorderService.AppendToEndAsync(cashAccount.LedgerAccount, p => p.TenantId == cashAccount.TenantId &&
                p.ParentId == cashAccount.LedgerAccount.ParentId, cancellationToken);


        await _attachmentService.RestoreAllForOwnerAsync(
            AttachmentOwnerType.MonetaryAccount, cashAccount.Id, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
