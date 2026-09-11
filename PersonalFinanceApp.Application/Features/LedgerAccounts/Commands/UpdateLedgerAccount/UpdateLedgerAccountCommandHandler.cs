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

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.UpdateLedgerAccount;

public class UpdateLedgerAccountCommandHandler : IRequestHandler<UpdateLedgerAccountCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLedgerAccountCommandHandler(
                IApplicationDbContext context,
                ICurrentUserService currentUser,
                IUnitOfWork unitOfWork)
    {
        _context = context;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateLedgerAccountCommand request, CancellationToken cancellationToken)
    {


        var ledgerAccount = await _context.LedgerAccounts
                .FirstOrDefaultAsync(r => r.Id == request.LedgerAccountId, cancellationToken)
                ?? throw new NotFoundException(nameof(LedgerAccount), request.LedgerAccountId);

        ledgerAccount.UpdateLedgerAccount(request.Name, _currentUser.UserId, request.Description);


        await _unitOfWork.SaveChangesAsync(cancellationToken);

    }
}
