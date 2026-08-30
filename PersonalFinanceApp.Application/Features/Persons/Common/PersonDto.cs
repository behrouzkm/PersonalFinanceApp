using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.Persons.Common;

public class PersonDto
{
    public Guid Id {get;set;}
    public PersonType PersonType { get;  set; }
    public string DisplayName { get;  set; } = string.Empty!;
    public Guid LedgerAccountId { get;  set; } 
    public DateOnly OpeningDate { get;  set; }
    public decimal InitialBalance { get;  set; }
    public decimal CurrentBalance { get;  set; }
    public decimal? CreditLimit { get;  set; }
    public Guid? OpeningAccountingDocumentId { get; set; }
    public int CurrencyId { get;  set; }
    public string CurrencyName { get;  set; } = null!;
    public string CurrencySymbol {get;set;} = null!;
    public int DisplayOrder { get;  set; }
    public string? Email { get;  set; }
    public string? MobileNumber { get;  set; }
    public string? TelNumber { get;  set; }
}
