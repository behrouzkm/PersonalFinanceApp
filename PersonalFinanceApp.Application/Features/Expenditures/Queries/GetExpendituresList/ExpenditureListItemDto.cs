using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Features.Expenditures.Queries.GetExpendituresList;

public class ExpenditureListItemDto
{
    public Guid AccountingDocumentId { get; set; }
    public DateOnly DocumentDate { get; set; }
    public byte CurrencyId { get; set; }
    public string? Description { get; set; }

    // Sum of the expense (debit) lines - equal to the sum of payment (credit) entries
    // by construction, since every document is validated to balance before it's saved.
    public decimal TotalAmount { get; set; }
}
