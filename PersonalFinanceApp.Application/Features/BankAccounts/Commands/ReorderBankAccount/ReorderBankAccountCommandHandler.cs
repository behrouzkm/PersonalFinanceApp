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

namespace PersonalFinanceApp.Application.Features.BankAccounts.Commands.ReorderBankAccount;

public class ReorderBankAccountCommandHandler : IRequestHandler<ReorderBankAccountCommand>
{
    private readonly IReorderService _reorderService;

    public ReorderBankAccountCommandHandler(IReorderService reorderService)
    {
        _reorderService = reorderService;
    }

    public async Task Handle(ReorderBankAccountCommand request, CancellationToken cancellationToken)
    => await _reorderService.ReorderAsync<BankAccount>(
        p => p.Id == request.BankAccountId, request.BankAccountId, request.NewDisplayOrder, cancellationToken);
}
