using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Features.Common;

namespace PersonalFinanceApp.Application.Features.Incomes.Common;

public interface IIncomeRequest
{
    DateOnly DocumentDate { get; }
    byte CurrencyId { get; }
    string? Description { get; }
    List<AccountingEntryDto> IncomeLedgerAccountLines { get; }
    List<MonetaryAccountEntryDto> MonetaryAccountEntries { get; }

}
