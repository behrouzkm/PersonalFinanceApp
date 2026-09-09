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

namespace PersonalFinanceApp.Application.Features.BankAccounts.Commands.RestoreBankAccount;

public class RestoreBankAccountCommandHandler : IRequestHandler<RestoreBankAccountCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IReorderService _reorderService;
    private readonly IAttachmentService _attachmentService;

    public RestoreBankAccountCommandHandler(
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

    public async Task Handle(RestoreBankAccountCommand request, CancellationToken cancellationToken)
    {
        var bankAccount = await _context.BankAccounts
            .IgnoreQueryFilters()
            .Include(p => p.LedgerAccount)
            .Include(p => p.OpeningAccountingDocument)
                .ThenInclude(p => p!.Entries)
            .FirstOrDefaultAsync(d => d.Id == request.BankAccountId
                && d.TenantId == _currentUser.TenantId, cancellationToken)
            ?? throw new NotFoundException(nameof(BankAccount), request.BankAccountId);

        bankAccount.Restore(_currentUser.UserId);
        bankAccount.LedgerAccount.Restore(_currentUser.UserId);

        if (bankAccount.OpeningAccountingDocument is not null)
            bankAccount.OpeningAccountingDocument.Restore(_currentUser.UserId);


        await _reorderService.AppendToEndAsync(bankAccount, p => p.TenantId == bankAccount.TenantId, cancellationToken);
        await _reorderService.AppendToEndAsync(bankAccount.LedgerAccount, p => p.TenantId == bankAccount.TenantId &&
                p.ParentId == bankAccount.LedgerAccount.ParentId, cancellationToken);



        await _attachmentService.RestoreAllForOwnerAsync(
            AttachmentOwnerType.MonetaryAccount, bankAccount.Id, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
