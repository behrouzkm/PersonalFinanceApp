using System.ComponentModel.DataAnnotations;
using PersonalFinanceApp.Domain.Common;
using PersonalFinanceApp.Domain.Errors;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Domain.Entities;

public abstract class MonetaryAccount : BaseAuditableEntity, IFundSource, IReorderable
{
    public string DisplayName { get; private set; } = string.Empty;

    // Foreign key to the related ledger account
    public Guid LedgerAccountId { get; private set; }
    public LedgerAccount LedgerAccount { get; private set; } = null!;

    public DateOnly OpeningDate { get; private set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    public decimal InitialBalance { get; private set; }


    public Guid? OpeningAccountingDocumentId { get; private set; }
    public AccountingDocument? OpeningAccountingDocument { get; private set; }

    public decimal CurrentBalance { get; private set; }

    public decimal? CreditLimit { get; private set; } // Optional credit limit for accounts that can go negative

    // Foreign key to the related currency
    public int CurrencyId { get; private set; }
    public Currency Currency { get; private set; } = null!;

    public int DisplayOrder { get; private set; }


    [Timestamp]
    public byte[] RowVersion { get; set; } = default!;



    protected MonetaryAccount()
    {
        CreditLimit = 0; // Default to 0 if not provided
    }

    protected MonetaryAccount(
        string displayName,
        Guid ledgerAccountId,
        int currencyId,
        DateOnly openingDate,
        decimal initialBalance,
        int displayOrder,
        Guid tenantId,
        Guid createdBy,
        decimal creditLimit = 0,
        string? description = null,
        Guid? openingAccountingDocumentId = null) : base(tenantId, createdBy, description)
    {
        SetDisplayName(displayName);
        SetLedgerAccountId(ledgerAccountId);
        SetCurrencyId(currencyId);
        SetOpeningDate(openingDate);
        SetCreditLimit(creditLimit);
        SetInitialBalance(initialBalance);
        SetDisplayOrder(displayOrder);

        OpeningAccountingDocumentId = openingAccountingDocumentId;
    }

    protected void UpdateMonetaryAccount(
        string displayName,
        int currencyId,
        DateOnly openingDate,
        decimal initialBalance,
        Guid modifiedBy,
        decimal creditLimit,
        string? description)
    {
        SetDisplayName(displayName);
        SetCurrencyId(currencyId);
        SetOpeningDate(openingDate);

        SetCreditLimit(creditLimit);
        UpdateInitialBalance(initialBalance);

        SetDescription(description);

        UpdateAudit(modifiedBy);
    }

    public void UpdateOpeningAccountingDocumentId(Guid? openingAccountingDocumentId, Guid modifiedBy)
    {
        OpeningAccountingDocumentId = openingAccountingDocumentId;
        UpdateAudit(modifiedBy);
    }

    private void SetDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new DomainException(DomainErrors.MonetaryAccount.DisplayNameRequired);

        DisplayName = displayName.Trim();
    }

    private void SetLedgerAccountId(Guid ledgerAccountId)
    {
        if (ledgerAccountId == Guid.Empty)
            throw new DomainException(DomainErrors.MonetaryAccount.LedgerAccountRequired);

        LedgerAccountId = ledgerAccountId;
    }

    private void SetCurrencyId(int currencyId)
    {
        if (currencyId == 0)
            throw new DomainException(DomainErrors.MonetaryAccount.CurrencyRequired);

        CurrencyId = currencyId;
    }

    private void SetCreditLimit(decimal? creditLimit)
    {
        if (creditLimit.HasValue == false)
            creditLimit = 0; // Default to 0 if not provided

        else if (creditLimit.Value < 0)
            throw new DomainException(DomainErrors.MonetaryAccount.CreditLimitCannotBeNegative);


        if (CurrentBalance < 0 && creditLimit.Value < Math.Abs(CurrentBalance))
            throw new DomainException(DomainErrors.MonetaryAccount.CreditLimitCannotBeLessThanCurrentNegativeBalance);

        CreditLimit = creditLimit;
    }

    private void SetOpeningDate(DateOnly openingDate)
    {
        if (openingDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException(DomainErrors.MonetaryAccount.OpeningDateCannotBeInFuture);

        OpeningDate = openingDate;
    }
    private void SetInitialBalance(decimal initialBalance)
    {
        if (initialBalance < CreditLimit * -1) // Ensure initial balance is not less than negative credit limit
            throw new DomainException(DomainErrors.MonetaryAccount.InitialBalanceCannotBeLessThanCreditLimit);

        InitialBalance = initialBalance;
        CurrentBalance = initialBalance; // Set current balance to initial balance when creating the account
    }

    // UpdateDetails calls this instead — preserves everything AdjustBalance has accrued.
    private void UpdateInitialBalance(decimal newInitialBalance)
    {
        var delta = newInitialBalance - InitialBalance;
        InitialBalance = newInitialBalance;
        CurrentBalance += delta;
    }

    public void SetDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
            throw new DomainException(DomainErrors.MonetaryAccount.DisplayOrderCannotBeNegative);

        DisplayOrder = displayOrder;
    }


    public bool CanWithdraw(decimal amount)
    {
        return amount <= CurrentBalance + CreditLimit;
    }

    public void AdjustBalance(decimal amount)
    {
        decimal newBalance = CurrentBalance + amount;
        if (newBalance < CreditLimit * -1) // Ensure current balance does not go below negative credit limit
            throw new DomainException(DomainErrors.MonetaryAccount.CurrentBalanceCannotBeLessThanCreditLimit);

        CurrentBalance = newBalance;
    }



}
