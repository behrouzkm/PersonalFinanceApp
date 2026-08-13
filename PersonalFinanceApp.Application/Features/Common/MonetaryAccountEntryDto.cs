using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Common.Interfaces;

namespace PersonalFinanceApp.Application.Features.Common;

/// <summary>
/// DTO for a payment made from a monetary account (bank account, cash, etc.) to pay for an expenditure.
/// </summary>
public class MonetaryAccountEntryDto : IPaymentDto
{
    public Guid? AccountingEntryId { get; set; }
    public Guid MonetaryLedgerAccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }

    public Guid FundSourceId => MonetaryLedgerAccountId;
}
