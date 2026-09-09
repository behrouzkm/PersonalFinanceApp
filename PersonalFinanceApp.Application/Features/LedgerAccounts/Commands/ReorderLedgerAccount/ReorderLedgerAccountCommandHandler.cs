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

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.ReorderLedgerAccount;

public class ReorderLedgerAccountCommandHandler : IRequestHandler<ReorderLedgerAccountCommand>
{
    private readonly IReorderService _reorderService;

    public ReorderLedgerAccountCommandHandler(IReorderService reorderService)
    {
        _reorderService = reorderService;
    }

    public async Task Handle(ReorderLedgerAccountCommand request, CancellationToken cancellationToken)
    => await _reorderService.ReorderAsync<LedgerAccount>(
        p => p.Id == request.LedgerAccountId, request.LedgerAccountId, request.NewDisplayOrder, cancellationToken,
            p => p.ParentId == request.ParentId);
}
