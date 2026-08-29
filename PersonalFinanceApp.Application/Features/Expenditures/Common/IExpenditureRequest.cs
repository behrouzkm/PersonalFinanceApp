using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Features.Common;

namespace PersonalFinanceApp.Application.Features.Expenditures.Common;

// Shared shape between CreateExpenditureCommand and UpdateExpenditureCommand,
// so their validators can share one rule set instead of duplicating it.
public interface IExpenditureRequest
{
    DateOnly DocumentDate { get; }
    int CurrencyId { get; }
    string? Description { get; }
    List<AccountingEntryDto> ExpenditureLedgerAccountLines { get; }
    List<MonetaryAccountEntryDto> MonetaryAccountEntries { get; }
    List<PersonPaymentDto> PersonPaymentEntries { get; }


}
