using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.Persons.Commands.DeletePerson;

public class DeletePersonCommand : IRequest
{
    public Guid Id {get;set;}

}
