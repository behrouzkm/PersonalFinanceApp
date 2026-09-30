using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.Persons.Common;

public class PersonOptionDto
{
    public Guid Id { get; set; }
    public Guid LedgerAccountId { get; set; }
    public PersonType PersonType { get; set; }
    public string DisplayName { get; set; } = null!;
    public decimal CurrentBalance { get; set; }
    public string CurrencyName { get; set; } = null!;
    public string CurrencySymbol { get; set; } = null!;
    public int CurrencyDecimalPlaces { get; set; } = 2;
}
