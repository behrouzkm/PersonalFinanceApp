using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Features.Expenditures.Common;

// Shared shape between CreateExpenditureCommand and UpdateExpenditureCommand,
// so their validators can share one rule set instead of duplicating it.
public interface IExpenditureRequest
{
    DateOnly DocumentDate { get; }
    byte CurrencyId { get; }
    List<ExpenditureLineDto> ExpenditureLines { get; }
    List<MonetaryAccountPaymentDto> MonetaryAccountPayments { get; }
    List<PersonPaymentDto> PersonPayments { get; }

    string? Description { get; }
}
