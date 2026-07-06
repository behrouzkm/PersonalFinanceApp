using System.ComponentModel.DataAnnotations;
using PersonalFinanceApp.Domain.Common;
using PersonalFinanceApp.Domain.Enums;
using PersonalFinanceApp.Domain.Errors;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Domain.Entities;

public class AccountingDocument : BaseAuditableEntity, IConcurrencyAware
{
    // DocumentType as a Bitwise Flag to support multiple natures (e.g., Expense | Person)
    public DocumentType DocumentType { get; private set; }
    public DateOnly DocumentDate { get; private set; }


    public byte CurrencyId { get; private set; }
    public Currency Currency { get; private set; } = null!;


    private readonly List<AccountingEntry> _entries = new();
    public IReadOnlyCollection<AccountingEntry> Entries => _entries.AsReadOnly();

    [Timestamp]
    public byte[] RowVersion { get; set; } = default!;

    private AccountingDocument() { }

    public AccountingDocument(DocumentType documentType, DateOnly documentDate, byte currencyId,
                                Guid tenantId, Guid createdBy, string? description = null)
        : base(tenantId, createdBy, description)
    {
        SetDocumentType(documentType);
        SetDocumentDate(documentDate);
        SetCurrencyId(currencyId);
    }

    // We pass AccountCategory (Enum) instead of the whole LedgerAccount entity
    public void AddEntry(Guid accountId, decimal debit, decimal credit, string description)
    {
        ValidateEntryConsistency(accountId, description);

        var entry = new AccountingEntry(this.Id, accountId, debit, credit, description);
        _entries.Add(entry);
    }

    public void RemoveEntry(AccountingEntry entry)
    {
        if (entry is null)
            throw new DomainException(DomainErrors.AccountingDocument.EntryRequired);

        _entries.Remove(entry);
    }

    // Call before posting to any MonetaryAccount or Person to enforce that the account's
    // native currency matches this document's currency.
    public void EnsureCurrencyMatches(byte accountCurrencyId)
    {
        if (accountCurrencyId != CurrencyId)
            throw new DomainException(DomainErrors.AccountingDocument.CurrencyMismatch);
    }

    private void ValidateEntryConsistency(Guid accountId, string description)
    {
        if (_entries.Any(e => e.LedgerAccountId == accountId && e.Description == description.Trim()))
        {
            throw new DomainException(DomainErrors.AccountingDocument.DuplicateEntryNotAllowed);
        }
    }


    public void SetDocumentType(DocumentType documentType)
    {
        if (documentType == 0)
            throw new DomainException(DomainErrors.AccountingDocument.DocumentTypeRequired);
        DocumentType = documentType;
    }

    public void SetDocumentDate(DateOnly documentDate)
    {
        if (documentDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException(DomainErrors.AccountingDocument.DocumentDateCannotBeInFuture);
        DocumentDate = documentDate;
    }

    public void SetCurrencyId(byte currencyId)
    {
        if (currencyId == 0)
            throw new DomainException(DomainErrors.AccountingDocument.CurrencyRequired);
        CurrencyId = currencyId;
    }
}
