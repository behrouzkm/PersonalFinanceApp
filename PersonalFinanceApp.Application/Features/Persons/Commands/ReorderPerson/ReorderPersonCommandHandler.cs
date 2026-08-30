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

namespace PersonalFinanceApp.Application.Features.Persons.Commands.ReorderPerson;

public class ReorderPersonCommandHandler : IRequestHandler<ReorderPersonCommand>
{
    private readonly IReorderService _reorderService;

    public ReorderPersonCommandHandler(IReorderService reorderService)
    {
        _reorderService = reorderService;
    }

    public async Task Handle(ReorderPersonCommand request, CancellationToken cancellationToken)
    => await _reorderService.ReorderAsync<Person>(
        p => p.Id == request.Id, request.Id, request.NewDisplayOrder, cancellationToken);
}
