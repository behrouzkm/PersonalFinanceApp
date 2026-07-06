using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.Expenditures.Common;

// Shared by CreateExpenditure and UpdateExpenditure - one "what was it spent on" line.
public class ExpenditureLineDto
{
    public Guid? LineId {get;set;}
    public Guid ExpenseLedgerAccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}
