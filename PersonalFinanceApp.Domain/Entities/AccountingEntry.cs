using PersonalFinanceApp.Domain.Common;
using PersonalFinanceApp.Domain.Errors;
using PersonalFinanceApp.Domain.Enums; // Assume AccountCategory enum is here

namespace PersonalFinanceApp.Domain.Entities;

public class AccountingEntry : BaseDetailEntity
{
    public Guid AccountingDocumentId { get; private set; }
    public AccountingDocument Document { get; private set; } = null!;

    public Guid LedgerAccountId { get; private set; }
    public LedgerAccount LedgerAccount { get; private set; } = null!;

    public decimal Debit { get; private set; }
    public decimal Credit { get; private set; }

    public byte CurrencyId { get; private set; }
    public Currency Currency { get; private set; } = null!;

    private AccountingEntry() { }

    // Simplified constructor: Only accepts essential IDs
    internal AccountingEntry(Guid accountingDocumentId, Guid ledgerAccountId, decimal debit, decimal credit, string? description)
    {
        SetDocumentId(accountingDocumentId);
        SetLedgerAccountId(ledgerAccountId);
        SetAmounts(debit, credit);
        SetDescription(description ?? string.Empty);
    }

    public void UpdateEntry(decimal debit, decimal credit, string? description)
    {
        SetAmounts(debit, credit);
        SetDescription(description ?? string.Empty);
    }

    public void SetDocumentId(Guid documentId)
    {
        if (documentId == Guid.Empty)
            throw new DomainException(DomainErrors.AccountingEntry.DocumentRequired);
        AccountingDocumentId = documentId;
    }

    public void SetLedgerAccountId(Guid ledgerAccountId)
    {
        if (ledgerAccountId == Guid.Empty)
            throw new DomainException(DomainErrors.AccountingEntry.LedgerAccountRequired);
        LedgerAccountId = ledgerAccountId;
    }

    public void SetAmounts(decimal debit, decimal credit)
    {
        if (debit < 0 || credit < 0)
            throw new DomainException(DomainErrors.AccountingEntry.NegativeAmountNotAllowed);

        if (debit == 0 && credit == 0)
            throw new DomainException(DomainErrors.AccountingEntry.DebitAndCreditCannotBothBeZero);

        if (debit > 0 && credit > 0)
            throw new DomainException(DomainErrors.AccountingEntry.CannotBeBothDebitAndCredit);

        Debit = debit;
        Credit = credit;
    }

    public void SetCurrencyId(byte currencyId)
    {
        if (currencyId == 0)
            throw new DomainException(DomainErrors.AccountingEntry.CurrencyRequired);
        CurrencyId = currencyId;
    }
}
