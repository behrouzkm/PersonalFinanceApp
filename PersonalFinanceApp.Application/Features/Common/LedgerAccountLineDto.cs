using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.Common;


public class AccountingEntryDto
{
    // null: for a new row to insert
    // set: when the row already exists in the document and needs to be updated
    // untouched: when the row already exists in the document and doesn't need to be updated
    public Guid? AccountingEntryId { get; set; }
    public Guid LedgerAccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}
