using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Application.Common.Services;

public class OpeningBalanceService : IOpeningBalanceService
{
    private readonly IApplicationDbContext _context;
    private readonly IAccountingLookupService _lookupService;
    private readonly ILedgerBalanceValidationService _ledgerBalance;
    private readonly ICurrentUserService _currentUser;

    public OpeningBalanceService(
                IApplicationDbContext context,
                IAccountingLookupService lookupService,
                ILedgerBalanceValidationService ledgerBalance,
                ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
        _ledgerBalance = ledgerBalance;
        _lookupService = lookupService;
    }


    public async Task<(LedgerAccount, Guid?)> CreateAsync(
        Guid parentLedgerAccountId, AccountCategory category, DocumentType documentType,
        string displayName, DateOnly openingDate, int currencyId,
        decimal initialBalance, decimal? creditLimit, string? description,
        CancellationToken cancellationToken)
    {
        if (creditLimit.HasValue && initialBalance < 0 && creditLimit.Value < Math.Abs(initialBalance))
            throw new BusinessRuleException(ApplicationErrorCodes.FundSource.InitialDebtExceedsCreditLimit);

        var accountType = await _context.AccountTypes
            .FirstOrDefaultAsync(r => r.Category == category, cancellationToken)
            ?? throw new NotFoundException(nameof(AccountType), category);

        var parent = await _context.LedgerAccounts
            .FirstOrDefaultAsync(r => r.Id == parentLedgerAccountId, cancellationToken)
            ?? throw new NotFoundException(nameof(LedgerAccount), parentLedgerAccountId);

        if (parent.AccountTypeId != accountType.Id)
            throw new BusinessRuleException(ApplicationErrorCodes.FundSource.InvalidParentLedgerAccount);

        var ledgerAccount = new LedgerAccount(
            accountType.Id, displayName, _currentUser.TenantId, _currentUser.UserId, description);

        _context.LedgerAccounts.Add(ledgerAccount);
        parent.AddChild(ledgerAccount);

        Guid? openingDocId = null;

        if (initialBalance != 0)
        {
            var equityAccount = await _lookupService.GetOpeningBalanceEquityLedgerAccount(category, cancellationToken)
                ?? throw new BusinessRuleException(ApplicationErrorCodes.FundSource.InvalidOpeningAccountEquityLedgerAccount);

            var doc = new AccountingDocument(
                documentType | DocumentType.OpeningBalance,
                openingDate, currencyId, _currentUser.TenantId, _currentUser.UserId, description);

            var amount = Math.Abs(initialBalance);
            var (debit, credit) = initialBalance > 0 ? (amount, 0m) : (0m, amount);

            doc.AddEntry(ledgerAccount.Id, debit, credit, description, _currentUser.UserId);
            doc.AddEntry(equityAccount.Id, credit, debit, description, _currentUser.UserId);

            _context.AccountingDocuments.Add(doc);
            openingDocId = doc.Id;
        }

        return (ledgerAccount, openingDocId);
    }

    public async Task<Guid?> ReconcileAsync(
        IFundSource fundSource, Guid? existingOpeningDocumentId,
        decimal oldInitialBalance, decimal? oldCreditLimit, int oldCurrencyId,
        AccountCategory category, DocumentType documentType, string? description,
        CancellationToken cancellationToken)
    {
        // fundSource already reflects the NEW InitialBalance/CreditLimit/OpeningDate/CurrencyId.
        var newInitialBalance = fundSource.InitialBalance;

        if (fundSource.CreditLimit.HasValue && newInitialBalance < 0
            && fundSource.CreditLimit.Value < Math.Abs(newInitialBalance))
            throw new BusinessRuleException(ApplicationErrorCodes.FundSource.InitialDebtExceedsCreditLimit);

        // Guard: currency is only mutable while the ledger account has no real
        // history beyond its own opening document — same principle as
        // LedgerAccount.HasBeenUsedInEntries.
        if (fundSource.CurrencyId != oldCurrencyId)
        {
            var hasNonOpeningEntries = await _context.AccountingEntries.AnyAsync(e =>
                e.LedgerAccountId == fundSource.LedgerAccountId &&
                (existingOpeningDocumentId == null || e.AccountingDocumentId != existingOpeningDocumentId.Value),
                cancellationToken);

            if (hasNonOpeningEntries)
                throw new BusinessRuleException(ApplicationErrorCodes.FundSource.CannotChangeCurrencyWithExistingTransactions);
        }

        if (existingOpeningDocumentId is null && newInitialBalance == 0)
            return null;

        if (existingOpeningDocumentId is null)
        {
            var hasEarlierEntries = await (
                from document in _context.AccountingDocuments
                join entry in _context.AccountingEntries on document.Id equals entry.AccountingDocumentId
                where entry.LedgerAccountId == fundSource.LedgerAccountId
                      && document.DocumentDate < fundSource.OpeningDate
                select document.Id).AnyAsync(cancellationToken);

            if (hasEarlierEntries)
                throw new BusinessRuleException(ApplicationErrorCodes.FundSource.OpeningDateCannotBeAfterExistingTransactions);

            var equityAccount = await _lookupService.GetOpeningBalanceEquityLedgerAccount(category, cancellationToken)
                ?? throw new BusinessRuleException(ApplicationErrorCodes.FundSource.InvalidOpeningAccountEquityLedgerAccount);

            var doc = new AccountingDocument(
                documentType | DocumentType.OpeningBalance,
                fundSource.OpeningDate, fundSource.CurrencyId, _currentUser.TenantId, _currentUser.UserId, description);

            var amount = Math.Abs(newInitialBalance);
            var (debit, credit) = newInitialBalance > 0 ? (amount, 0m) : (0m, amount);

            doc.AddEntry(fundSource.LedgerAccountId, debit, credit, description, _currentUser.UserId);
            doc.AddEntry(equityAccount.Id, credit, debit, description, _currentUser.UserId);

            _context.AccountingDocuments.Add(doc);

            await _ledgerBalance.ValidateAsync(
                fundSource, fundSource.OpeningDate, debit, credit, replacingEntryId: null, cancellationToken);

            return doc.Id;
        }

        var existingDoc = await _context.AccountingDocuments
            .FirstOrDefaultAsync(r => r.Id == existingOpeningDocumentId, cancellationToken)
            ?? throw new NotFoundException(nameof(AccountingDocument), existingOpeningDocumentId);

        if (newInitialBalance != 0)
        {
            var hasEarlierEntries = await (
                from document in _context.AccountingDocuments
                join entry in _context.AccountingEntries on document.Id equals entry.AccountingDocumentId
                where document.Id != existingOpeningDocumentId
                      && entry.LedgerAccountId == fundSource.LedgerAccountId
                      && document.DocumentDate < fundSource.OpeningDate
                select document.Id).AnyAsync(cancellationToken);

            if (hasEarlierEntries)
                throw new BusinessRuleException(ApplicationErrorCodes.FundSource.OpeningDateCannotBeAfterExistingTransactions);
        }

        var headerChanged = fundSource.OpeningDate != existingDoc.DocumentDate
                          || fundSource.CurrencyId != existingDoc.CurrencyId
                          || description != existingDoc.Description;

        if (headerChanged)
            existingDoc.UpdateAccountingDocument(fundSource.OpeningDate, fundSource.CurrencyId, _currentUser.UserId, description);

        var fundSourceEntry = existingDoc.Entries.FirstOrDefault(e => e.LedgerAccountId == fundSource.LedgerAccountId)
            ?? throw new BusinessRuleException(ApplicationErrorCodes.FundSource.InvalidOpeningAccountEquityLedgerAccount);

        var balanceDecreased = newInitialBalance < oldInitialBalance;
        var creditLimitChanged = fundSource.CreditLimit != oldCreditLimit;

        if (newInitialBalance != 0 && newInitialBalance != oldInitialBalance)
        {
            var amount = Math.Abs(newInitialBalance);
            var (debit, credit) = newInitialBalance > 0 ? (amount, 0m) : (0m, amount);

            if (balanceDecreased)
                await _ledgerBalance.ValidateAsync(
                    fundSource, fundSource.OpeningDate, debit, credit, fundSourceEntry.Id, cancellationToken);

            foreach (var item in existingDoc.Entries)
            {
                if (item.LedgerAccountId == fundSource.LedgerAccountId)
                    item.SetAmounts(debit, credit);
                else
                    item.SetAmounts(credit, debit);

                item.SetDescription(description);
                item.UpdateAudit(_currentUser.UserId);
            }
        }
        else if (newInitialBalance == 0)
        {
            await _ledgerBalance.ValidateRemovalAsync(fundSource, fundSourceEntry.Id, cancellationToken);
            _context.AccountingDocuments.Remove(existingDoc);
            return null;
        }
        else if (creditLimitChanged)
        {
            // InitialBalance unchanged, entries untouched — but a shrinking CreditLimit
            // still needs the full chronological walk, not just SetCreditLimit's snapshot check.
            await _ledgerBalance.ValidateAsync(
                fundSource, fundSource.OpeningDate, fundSourceEntry.Debit, fundSourceEntry.Credit,
                fundSourceEntry.Id, cancellationToken);
        }

        return existingDoc.Id;
    }
}
