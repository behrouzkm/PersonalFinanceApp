using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Common.Interfaces;

public interface IPaymentDto
{
    Guid? AccountingEntryId {get;}
    Guid FundSourceId {get;}
    decimal Amount{get;}
    string? Description{get;}
}
