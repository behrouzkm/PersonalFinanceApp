using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Application.Common.Interfaces;

public interface IOpeningBalanceService
{
    /// Create-time: ledger account under a category parent + optional opening document.
    Task<(LedgerAccount LedgerAccount, Guid? OpeningDocumentId)> CreateAsync(
        Guid parentLedgerAccountId,
        AccountCategory category,
        DocumentType documentType,
        string displayName,
        DateOnly openingDate,
        int currencyId,
        decimal initialBalance,
        decimal? creditLimit,
        string? description,
        CancellationToken cancellationToken);


    /// Update-time. `fundSource` MUST already reflect the proposed new state
    /// (call entity.UpdateDetails(...) before calling this) — ValidateAsync/
    /// ValidateRemovalAsync read CreditLimit/InitialBalance/OpeningDate off
    /// the reference you pass in, not off the database.
    Task<Guid?> ReconcileAsync(
        IFundSource fundSource, Guid? existingOpeningDocumentId, decimal oldInitialBalance,
        AccountCategory category, DocumentType documentType, string? description,
        CancellationToken cancellationToken);

}
