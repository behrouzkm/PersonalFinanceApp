using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.Persons.Commands.DeletePerson;

public class DeletePersonCommand : IRequest
{
    public Guid PersonId { get; set; }

    
    // RowVersion is used for concurrency control to ensure that the document has not been modified by
    // another user since it was last retrieved.
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
