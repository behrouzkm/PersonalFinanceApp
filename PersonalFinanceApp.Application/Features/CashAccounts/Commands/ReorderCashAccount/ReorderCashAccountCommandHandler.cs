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

namespace PersonalFinanceApp.Application.Features.CashAccounts.Commands.ReorderCashAccount;

public class ReorderCashAccountCommandHandler : IRequestHandler<ReorderCashAccountCommand>
{
    private readonly IReorderService _reorderService;

    public ReorderCashAccountCommandHandler(IReorderService reorderService)
    {
        _reorderService = reorderService;
    }

    public async Task Handle(ReorderCashAccountCommand request, CancellationToken cancellationToken)
    => await _reorderService.ReorderAsync<CashAccount>(
        p => p.Id == request.CashAccountId, request.CashAccountId, request.NewDisplayOrder, cancellationToken);
}
