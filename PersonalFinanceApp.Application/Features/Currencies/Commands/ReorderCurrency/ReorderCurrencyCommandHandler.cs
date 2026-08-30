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

namespace PersonalFinanceApp.Application.Features.Currencies.Commands.ReorderCurrency;

public class ReorderCurrencyCommandHandler : IRequestHandler<ReorderCurrencyCommand>
{
    private readonly IReorderService _reorderService;

    public ReorderCurrencyCommandHandler(IReorderService reorderService)
    {
        _reorderService = reorderService;
    }

    public async Task Handle(ReorderCurrencyCommand request, CancellationToken cancellationToken)
    => await _reorderService.ReorderAsync<Currency>(
        p => p.Id == request.Id, request.Id, request.NewDisplayOrder, cancellationToken);
}
