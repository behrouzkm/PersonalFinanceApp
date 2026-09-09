using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.Persons.Commands.RestorePerson;

public class RestorePersonCommand : IRequest
{
    public Guid PersonId { get; set; }
}
