using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.Persons.Commands.ReorderPerson;

public class ReorderPersonCommand : IRequest
{
    public Guid Id { get; set; }
    public int NewDisplayOrder { get; set; }
}
