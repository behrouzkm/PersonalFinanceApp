using PersonalFinanceApp.Domain.Common;
using PersonalFinanceApp.Domain.Errors;

namespace PersonalFinanceApp.Domain.Entities;

public abstract class MonetaryAccount : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;

    // Foreign key to the related ledger account
    public Guid LedgerAccountId { get; private set; }
    public LedgerAccount LedgerAccount { get; private set; } = null!;

    public decimal InitialBalance { get; private set; }

    public decimal CurrentBalance { get; private set; }

    // Foreign key to the related currency
    public byte CurrencyId { get; private set; }
    public Currency Currency { get; private set; } = null!;

     public int DisplayOrder { get; private set; }

    public bool CanWithdraw(decimal amount) => CurrentBalance >= amount;


    protected MonetaryAccount() { }

    public MonetaryAccount(string name, Guid ledgerAccountId, byte currencyId, decimal initialBalance, int displayOrder,
                            Guid tenantId, Guid createdBy) : base(tenantId, createdBy)
    {
        SetName(name);
        SetLedgerAccountId(ledgerAccountId);
        SetCurrencyId(currencyId);
        SetInitialBalance(initialBalance);
        SetDisplayOrder(displayOrder);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(DomainErrors.MonetaryAccount.NameRequired);

        Name = name.Trim();
    }

    public void SetLedgerAccountId(Guid ledgerAccountId)
    {
        if (ledgerAccountId == Guid.Empty)
            throw new DomainException(DomainErrors.MonetaryAccount.LedgerAccountRequired);

        LedgerAccountId = ledgerAccountId;
    }

    public void SetCurrencyId(byte currencyId)
    {
        if (currencyId == 0)
            throw new DomainException(DomainErrors.MonetaryAccount.CurrencyRequired);

        CurrencyId = currencyId;
    }

    public void SetInitialBalance(decimal initialBalance)
    {
        if (initialBalance < 0)
            throw new DomainException(DomainErrors.MonetaryAccount.InitialBalanceCannotBeNegative);

        InitialBalance = initialBalance;
        CurrentBalance = initialBalance; // Set current balance to initial balance when creating the account
    }

    public void SetDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
            throw new DomainException(DomainErrors.MonetaryAccount.DisplayOrderCannotBeNegative);

        DisplayOrder = displayOrder;
    }

    public void UpdateCurrentBalance(decimal newBalance)
    {
        if (newBalance < 0)
            throw new DomainException(DomainErrors.MonetaryAccount.CurrentBalanceCannotBeNegative);

        CurrentBalance = newBalance;
    }

    public void AdjustBalance(decimal amount)
    {
        decimal newBalance = CurrentBalance + amount;
        if (newBalance < 0)
            throw new DomainException(DomainErrors.MonetaryAccount.CurrentBalanceCannotBeNegative);

        CurrentBalance = newBalance;
    }

}
