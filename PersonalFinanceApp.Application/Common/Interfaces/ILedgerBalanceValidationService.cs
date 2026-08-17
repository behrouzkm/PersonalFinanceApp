using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Application.Common.Interfaces
{

    public interface ILedgerBalanceValidationService
    {
        // replacingEntryId: null for a brand-new entry (Create, or a new row on Update);
        // the AccountingEntryId being edited in place for Update - excluded from the
        // "existing" set before the proposed new state is replayed.
        Task ValidateAsync(IFundSource fundSource, DateOnly newDocumentDate, decimal newDebit, decimal newCredit,
                            Guid? replacingEntryId, CancellationToken cancellationToken);


        // Validates a fund source's ledger as if the given entry were removed entirely,
        // with nothing replacing it. Used when a row is deleted outright, and for the
        // "old" side of a fund source change (the account/person a payment moved away from).
        Task ValidateRemovalAsync(IFundSource fundSource, Guid removingEntryId, CancellationToken cancellationToken);
    }
}
