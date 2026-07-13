using PersonalFinanceApp.Domain.Common;
using PersonalFinanceApp.Domain.Errors;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Domain.Entities;

public abstract class MonetaryAccount : BaseAuditableEntity, IFundSource
{
    public string DisplayName { get; private set; } = string.Empty;

    // Foreign key to the related ledger account
    public Guid LedgerAccountId { get; private set; }
    public LedgerAccount LedgerAccount { get; private set; } = null!;

    public decimal InitialBalance { get; private set; }

    public decimal CurrentBalance { get; private set; }

    public decimal? CreditLimit { get; private set; } // Optional credit limit for accounts that can go negative

    // Foreign key to the related currency
    public byte CurrencyId { get; private set; }
    public Currency Currency { get; private set; } = null!;

    public int DisplayOrder { get; private set; }




    protected MonetaryAccount()
    {
        CreditLimit = 0; // Default to 0 if not provided
    }

    public MonetaryAccount(string name, Guid ledgerAccountId, byte currencyId, decimal initialBalance, int displayOrder,
                            Guid tenantId, Guid createdBy, decimal creditLimit = 0, string? description = null) :
                                    base(tenantId, createdBy, description)
    {
        SetName(name);
        SetLedgerAccountId(ledgerAccountId);
        SetCurrencyId(currencyId);
        SetCreditLimit(creditLimit);
        SetInitialBalance(initialBalance);
        SetDisplayOrder(displayOrder);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(DomainErrors.MonetaryAccount.NameRequired);

        DisplayName = name.Trim();
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

    public void SetCreditLimit(decimal? creditLimit)
    {
        if (creditLimit.HasValue == false)
            creditLimit = 0; // Default to 0 if not provided

        else if (creditLimit.Value < 0)
            throw new DomainException(DomainErrors.MonetaryAccount.CreditLimitCannotBeNegative);


        if (CurrentBalance < 0 && creditLimit.Value < Math.Abs(CurrentBalance))
            throw new DomainException(DomainErrors.MonetaryAccount.CreditLimitCannotBeLessThanCurrentNegativeBalance);

        CreditLimit = creditLimit;
    }

    public void SetInitialBalance(decimal initialBalance)
    {
        if (initialBalance < CreditLimit.GetValueOrDefault(0) * -1) // Ensure initial balance is not less than negative credit limit
            throw new DomainException(DomainErrors.MonetaryAccount.InitialBalanceCannotBeLessThanCreditLimit);

        InitialBalance = initialBalance;
        CurrentBalance = initialBalance; // Set current balance to initial balance when creating the account
    }

    public void SetDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
            throw new DomainException(DomainErrors.MonetaryAccount.DisplayOrderCannotBeNegative);

        DisplayOrder = displayOrder;
    }

    public bool CanWithdraw(decimal amount)
    {
        return amount <= CurrentBalance + CreditLimit.GetValueOrDefault(0);
    }

    public void AdjustBalance(decimal amount)
    {
        decimal newBalance = CurrentBalance + amount;
        if (newBalance < CreditLimit.GetValueOrDefault(0) * -1) // Ensure current balance does not go below negative credit limit
            throw new DomainException(DomainErrors.MonetaryAccount.CurrentBalanceCannotBeLessThanCreditLimit);

        CurrentBalance = newBalance;
    }

}
