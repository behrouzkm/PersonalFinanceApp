using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens.Experimental;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Interfaces;
using PersonalFinanceApp.Domain.Services;

namespace PersonalFinanceApp.Application.Common.Services;

public class LedgerBalanceValidationService : ILedgerBalanceValidationService
{
    private readonly IApplicationDbContext _context;
    private readonly LedgerBalanceValidator _validator = new();

    public LedgerBalanceValidationService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task ValidateAsync(IFundSource fundSource, DateOnly newDocumentDate, decimal newDebit, decimal newCredit,
                                Guid? replacingEntryId, CancellationToken cancellationToken)
    {

        var entries = await LoadExistingEntriesAsync(fundSource, replacingEntryId, cancellationToken);

        entries.Add(new LedgerEntryPoint
        {
            EntryId = replacingEntryId ?? Guid.NewGuid(),
            DocumentDate = newDocumentDate,
            Debit = newDebit,
            Credit = newCredit
        });

        _validator.ValidateChronologicalBalance(fundSource, entries);
    }

    public async Task ValidateRemovalAsync(IFundSource fundSource, Guid removingEntryId, CancellationToken cancellationToken)
    {
        var entries = await LoadExistingEntriesAsync(fundSource, removingEntryId, cancellationToken);

        _validator.ValidateChronologicalBalance(fundSource, entries);
    }

    private async Task<List<LedgerEntryPoint>> LoadExistingEntriesAsync(IFundSource fundSource,
                                                    Guid? excludingEntryId, CancellationToken cancellationToken)
    {
        var entries = await _context.AccountingEntries
                .Where(r => r.LedgerAccountId == fundSource.LedgerAccountId)
                .Select(s => new LedgerEntryPoint
                {
                    DocumentDate = s.Document.DocumentDate,
                    CreatedAt = s.CreatedAt,
                    Debit = s.Debit,
                    Credit = s.Credit
                })
                .ToListAsync(cancellationToken);

        if (excludingEntryId.HasValue)
        {
            entries.RemoveAll(e => e.EntryId == excludingEntryId.Value);
        }

        return entries;
    }

    private sealed class LedgerEntryPoint : IledgerEntryPoint
    {
        public Guid EntryId { get; set; }

        public DateOnly DocumentDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }
    }
}
