using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.Incomes.Commands.UpdateIncome;

public class UpdateIncomeCommandHandler : IRequestHandler<UpdateIncomeCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAccountingLookupService _lookup;
    private readonly ILedgerBalanceValidationService _ledgerValidator;

    public UpdateIncomeCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IAccountingLookupService lookup,
        ILedgerBalanceValidationService ledgerValidator)
    {
        _context = context;
        _currentUser = currentUser;
        _lookup = lookup;
        _ledgerValidator = ledgerValidator;
    }

    public async Task Handle(UpdateIncomeCommand request, CancellationToken cancellationToken)
    {
        // load the document and its entries
        var document = await _context.AccountingDocuments
            .Include(d => d.Entries)
            .FirstOrDefaultAsync(d => d.Id == request.AccountingDocumentId, cancellationToken)
            ?? throw new NotFoundException(nameof(AccountingDocument), request.AccountingDocumentId);

        _context.AccountingDocuments.Entry(document).Property(d => d.RowVersion).OriginalValue = request.RowVersion;

        // set header properties
        document.UpdateAccountingDocument(request.DocumentDate, request.CurrencyId, _currentUser.UserId, request.Description);


        // load existing debit entries(deposits) and their ledger accounts
        var existingDebitEntries = document.Entries.Where(e => e.Debit > 0).ToList();
        var existingDebitLedgerAccountIds = existingDebitEntries.Select(e => e.LedgerAccountId).Distinct().ToList();

        var requestedMonetaryAccountIds = request.MonetaryAccountEntries
               .Select(e => e.MonetaryAccountId)
               .Distinct()
               .ToList();

        var monetaryAccountLookup = await _lookup.GetMonetaryAccountsAsync(
            requestedMonetaryAccountIds, existingDebitLedgerAccountIds, cancellationToken);

        // load requested credit entries (income ledger accounts)
        var requestedIncomeAccountIds = request.IncomeLedgerAccountLines.Select(l => l.LedgerAccountId).Distinct().ToList();
        var incomeAccounts = await _lookup.GetLedgerAccountsAsync(requestedIncomeAccountIds, cancellationToken);

        var existingEntriesById = document.Entries.ToDictionary(e => e.Id);

        var incomeLineEntryIds = request.IncomeLedgerAccountLines
            .Where(l => l.AccountingEntryId.HasValue)
            .Select(l => l.AccountingEntryId!.Value)
            .ToHashSet();

        // soft delete any credit entries (income ledger accounts) that are no longer on the document
        foreach (var oldEntry in document.Entries.Where(e => e.Credit > 0 && !incomeLineEntryIds.Contains(e.Id)).ToList())
        {
            document.RemoveEntry(oldEntry, _currentUser.UserId);
        }

        foreach (var line in request.IncomeLedgerAccountLines)
        {
            var account = incomeAccounts.TryGetValue(line.LedgerAccountId, out var acc)
                ? acc
                : throw new NotFoundException(nameof(LedgerAccount), line.LedgerAccountId);

            if (!account.IsPostingAccount)
                throw new BusinessRuleException(Application.Common.Errors.ApplicationErrorCodes.Income.IncomeAccountNotPostable,
                    account.Id, account.Name);

            // new row
            if (line.AccountingEntryId == null)
            {
                document.AddEntry(line.LedgerAccountId, 0, line.Amount, line.Description, _currentUser.UserId);
                account.MarkAsUsed();
                continue;
            }


            // update existing row
            var entry = existingEntriesById.TryGetValue(line.AccountingEntryId.Value, out var existingEntry)
                ? existingEntry
                : throw new BusinessRuleException(ApplicationErrorCodes.Income.EntryNotFoundOnDocument,
                    line.AccountingEntryId.Value);

            var changed = entry.LedgerAccountId != line.LedgerAccountId ||
                          entry.Credit != line.Amount ||
                          entry.Description != line.Description;

            if (!changed)
                continue;

            entry.UpdateEntry(line.LedgerAccountId, 0, line.Amount, line.Description);
            entry.UpdateAudit(_currentUser.UserId);

            account.MarkAsUsed();
        }


        var monetaryAccountEntryIds = request.MonetaryAccountEntries
            .Where(e => e.AccountingEntryId.HasValue)
            .Select(e => e.AccountingEntryId!.Value)
            .ToHashSet();

        // rows which user removed - reverse the deposits, then soft delete the entry
        foreach (var oldEntry in document.Entries.Where(e => e.Debit > 0 && !monetaryAccountEntryIds.Contains(e.Id)).ToList())
        {
            if (monetaryAccountLookup.ByLedgerAccountId.TryGetValue(oldEntry.LedgerAccountId, out var oldDepositAccount))
            {
                oldDepositAccount.AdjustBalance(-oldEntry.Debit);
                document.RemoveEntry(oldEntry, _currentUser.UserId);
            }
        }

        foreach (var deposit in request.MonetaryAccountEntries)
        {
            var monetaryAccount = monetaryAccountLookup.ById.TryGetValue(deposit.MonetaryAccountId, out var account)
                ? account
                : throw new NotFoundException(nameof(MonetaryAccount), deposit.MonetaryAccountId);


            if (deposit.AccountingEntryId == null)
            {
                // new row
                document.EnsureCurrencyMatches(monetaryAccount.CurrencyId);

                await _ledgerValidator.ValidateAsync(monetaryAccount, request.DocumentDate, deposit.Amount, 0,
                                    replacingEntryId: null, cancellationToken);

                document.AddEntry(monetaryAccount.LedgerAccountId, deposit.Amount, 0, deposit.Description, _currentUser.UserId);
                monetaryAccount.LedgerAccount.MarkAsUsed();
                monetaryAccount.AdjustBalance(deposit.Amount);
                continue;
            }


            var entry = existingEntriesById.TryGetValue(deposit.AccountingEntryId.Value, out var existingEntry)
                ? existingEntry
                : throw new BusinessRuleException(ApplicationErrorCodes.Income.EntryNotFoundOnDocument,
                    deposit.AccountingEntryId.Value);



            if (entry.LedgerAccountId != monetaryAccount.LedgerAccountId)
            {
                // user changed the deposit account, so we need to reverse the old deposit and apply the new one
                if (monetaryAccountLookup.ByLedgerAccountId.TryGetValue(entry.LedgerAccountId, out var oldDepositAccount))
                {
                    oldDepositAccount.AdjustBalance(-entry.Debit);

                    await _ledgerValidator.ValidateRemovalAsync(oldDepositAccount, entry.Id, cancellationToken);
                }

                document.EnsureCurrencyMatches(monetaryAccount.CurrencyId);

                await _ledgerValidator.ValidateAsync(
                    monetaryAccount, request.DocumentDate, deposit.Amount, 0, replacingEntryId: entry.Id, cancellationToken);

                entry.UpdateEntry(monetaryAccount.LedgerAccountId, deposit.Amount, 0, deposit.Description);
                entry.UpdateAudit(_currentUser.UserId);
                monetaryAccount.AdjustBalance(deposit.Amount);
                monetaryAccount.LedgerAccount.MarkAsUsed();

                continue;

            }


            // same deposit account, just update the amount and description
            var amountDelta = deposit.Amount - entry.Debit;
            if (amountDelta == 0 && entry.Description == deposit.Description)
                continue;

            await _ledgerValidator.ValidateAsync(
                monetaryAccount, request.DocumentDate, deposit.Amount, 0, replacingEntryId: entry.Id, cancellationToken);

            entry.SetAmounts(deposit.Amount, 0);
            entry.SetDescription(deposit.Description);
            entry.UpdateAudit(_currentUser.UserId);
            monetaryAccount.AdjustBalance(amountDelta);


        }

        await _context.SaveChangesAsync(cancellationToken);

    }

}

