using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.Persons.Commands.CreatePerson;

public class CreatePersonCommand : IRequest<Guid>
{
    public PersonType PersonType { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public Guid ParentLedgerId { get; set; }
    public DateOnly OpeningDate { get; set; }
    public decimal InitialBalance { get; set; }
    public decimal? CreditLimit { get; set; }
    public int CurrencyId { get; set; }
    public string? Email { get; set; }
    public string? MobileNumber { get; set; }
    public string? TelNumber { get; set; }
    public string? Description { get; set; }
}
