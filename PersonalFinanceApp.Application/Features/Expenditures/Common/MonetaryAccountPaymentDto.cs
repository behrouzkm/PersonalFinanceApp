using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Common.Interfaces;

namespace PersonalFinanceApp.Application.Features.Expenditures.Common;

public class MonetaryAccountPaymentDto : IPaymentDto
{
    public Guid? AccountingEntryId {get; set;}
    public Guid MonetaryAccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }

    public Guid FundSourceId => MonetaryAccountId;
}
