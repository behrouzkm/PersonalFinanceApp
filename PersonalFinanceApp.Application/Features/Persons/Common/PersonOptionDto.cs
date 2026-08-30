using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.Persons.Common;

public class PersonOptionDto
{
    public Guid Id { get; set; }
    public PersonType PersonType { get; set; }
    public string DisplayName { get; set; } = null!;
}
