using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Features.Expenditures.Common;

public class MonetaryAccountPaymentDto
{
    public Guid MonetaryAccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}
