using PersonalFinanceApp.Domain.Common;
using PersonalFinanceApp.Domain.Errors;
using PersonalFinanceApp.Domain.Enums; // Assume AccountCategory enum is here

namespace PersonalFinanceApp.Domain.Entities;

public class AccountingEntry : BaseAuditableEntity
{
    public Guid AccountingDocumentId { get; private set; }
    public AccountingDocument Document { get; private set; } = null!;

    public Guid LedgerAccountId { get; private set; }
    public LedgerAccount LedgerAccount { get; private set; } = null!;

    public decimal Debit { get; private set; }
    public decimal Credit { get; private set; }

    private AccountingEntry() { }

    public AccountingEntry(Guid accountingDocumentId, Guid ledgerAccountId, decimal debit, decimal credit,
                            string? description, Guid tenantId, Guid createdBy) : base(tenantId, createdBy, description)
    {
        SetDocumentId(accountingDocumentId);
        SetLedgerAccountId(ledgerAccountId);
        SetAmounts(debit, credit);
    }

    public void UpdateEntry(Guid ledgerAccountId, decimal debit, decimal credit, Guid modifiedBy, string? description)
    {
        SetLedgerAccountId(ledgerAccountId);
        SetAmounts(debit, credit);
        SetDescription(description);

        UpdateAudit(modifiedBy);
    }

    public void UpdateEntry(decimal debit, decimal credit, Guid modifiedBy)
    {
        SetAmounts(debit, credit);

        UpdateAudit(modifiedBy);
    }
    public void UpdateEntry(decimal debit, decimal credit, Guid modifiedBy, string? description)
    {
        SetAmounts(debit, credit);
        SetDescription(description);

        UpdateAudit(modifiedBy);
    }

    private void SetDocumentId(Guid documentId)
    {
        if (documentId == Guid.Empty)
            throw new DomainException(DomainErrors.AccountingEntry.DocumentRequired);
        AccountingDocumentId = documentId;
    }

    private void SetLedgerAccountId(Guid ledgerAccountId)
    {
        if (ledgerAccountId == Guid.Empty)
            throw new DomainException(DomainErrors.AccountingEntry.LedgerAccountRequired);
        LedgerAccountId = ledgerAccountId;
    }

    private void SetAmounts(decimal debit, decimal credit)
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

}
