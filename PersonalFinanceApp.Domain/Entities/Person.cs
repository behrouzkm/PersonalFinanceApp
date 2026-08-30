using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PersonalFinanceApp.Domain.Common;
using PersonalFinanceApp.Domain.Enums;
using PersonalFinanceApp.Domain.Errors;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Domain.Entities;

public class Person : BaseAuditableEntity, IFundSource, IReorderable
{


    public PersonType PersonType { get; private set; } = PersonType.Individual;

    public string DisplayName { get; private set; } = string.Empty!;


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

    public string? Email { get; private set; }

    public string? MobileNumber { get; private set; }

    public string? TelNumber { get; private set; }


    private Person() { }

    public Person(PersonType personType, string displayName, Guid ledgerAccountId, int currencyId, int displayOrder,
                    DateOnly openingDate, decimal initialBalance, Guid tenantId, Guid createdBy, string? email = null, string? mobileNumber = null,
                    string? telNumber = null, string? description = null, decimal? creditLimit = null,
                    Guid? openingAccountingDocumentId = null) : base(tenantId, createdBy, description)
    {
        PersonType = personType;
        SetDisplayName(displayName);
        SetOpeningDate(openingDate);
        SetLedgerAccountId(ledgerAccountId);
        SetCurrencyId(currencyId);
        SetCreditLimit(creditLimit);
        SetInitialBalance(initialBalance);
        SetDisplayOrder(displayOrder);
        SetEmail(email);
        SetMobileNumber(mobileNumber);
        SetTelNumber(telNumber);

        OpeningAccountingDocumentId = openingAccountingDocumentId;
    }


    public void UpdateDetails(PersonType personType, string displayName, int currencyId, DateOnly openingDate,
                                decimal initialBalance, Guid modifiedBy, decimal? creditLimit, string? email,
                                string? mobileNumber, string? telNumber, string? description)
    {
        PersonType = personType;
        SetDisplayName(displayName);
        SetCurrencyId(currencyId);
        SetOpeningDate(openingDate);

        SetEmail(email);
        SetMobileNumber(mobileNumber);
        SetTelNumber(telNumber);
        SetDescription(description);

        SetCreditLimit(creditLimit);
        UpdateInitialBalance(initialBalance);

        UpdateAudit(modifiedBy);
    }

    public void SetOpeningAccountingDocumentId(Guid? openingAccountingDocumentId, Guid modifiedBy)
    {
        OpeningAccountingDocumentId = openingAccountingDocumentId;
        UpdateAudit(modifiedBy);
    }


    private void SetDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new DomainException(DomainErrors.Person.DisplayNameRequired);

        DisplayName = displayName.Trim();
    }


    private void SetEmail(string? email)
    {
        if (!string.IsNullOrWhiteSpace(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new DomainException(DomainErrors.Person.InvalidEmailFormat);

        Email = email?.Trim().ToLower();
    }

    private void SetMobileNumber(string? mobileNumber)
    {
        if (string.IsNullOrWhiteSpace(mobileNumber))
        {
            MobileNumber = null;
            return;
        }

        // Remove common separators to validate the core number digits
        // Keeps the leading '+' if present
        var sanitized = mobileNumber.Trim();
        var digitsOnly = sanitized.Replace(" ", "").Replace("-", "");

        // Regex explanation:
        // ^\+?          : Optional leading '+'
        // \d{7,15}$ : Must start with a  digit, followed by 7 to 15 digits
        if (!Regex.IsMatch(digitsOnly, @"^\+?\d{7,15}$"))
        {
            throw new DomainException(DomainErrors.Person.InvalidMobileNumberFormat);
        }

        MobileNumber = digitsOnly;
    }

    private void SetTelNumber(string? telNumber)
    {
        if (string.IsNullOrWhiteSpace(telNumber))
        {
            TelNumber = null;
            return;
        }

        // Remove common separators to validate the core number digits
        // Keeps the leading '+' if present
        var sanitized = telNumber.Trim();
        var digitsOnly = sanitized.Replace(" ", "").Replace("-", "");

        // Regex explanation:
        // ^\+?          : Optional leading '+'
        // \d{7,15}$ : Must start with a  digit, followed by 7 to 15 digits
        if (!Regex.IsMatch(digitsOnly, @"^\+?\d{7,15}$"))
        {
            throw new DomainException(DomainErrors.Person.InvalidTelNumberFormat);
        }

        TelNumber = digitsOnly;
    }

    private void SetLedgerAccountId(Guid ledgerAccountId)
    {
        if (ledgerAccountId == Guid.Empty)
            throw new DomainException(DomainErrors.Person.LedgerAccountRequired);

        LedgerAccountId = ledgerAccountId;
    }

    private void SetCurrencyId(int currencyId)
    {
        if (currencyId == 0)
            throw new DomainException(DomainErrors.Person.CurrencyRequired);

        CurrencyId = currencyId;
    }


    public void SetDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
            throw new DomainException(DomainErrors.Person.DisplayOrderCannotBeNegative);

        DisplayOrder = displayOrder;
    }

    public void IncrementDisplayOrder() => DisplayOrder++;

    public void DecrementDisplayOrder()
    {
        if (DisplayOrder > 0)
            DisplayOrder--;
    }

    private void SetCreditLimit(decimal? creditLimit)
    {
        if (creditLimit.HasValue && creditLimit.Value < 0)
            throw new DomainException(DomainErrors.Person.CreditLimitCannotBeNegative);

        if (CurrentBalance < 0 && creditLimit.HasValue && creditLimit.Value < Math.Abs(CurrentBalance))
            throw new DomainException(DomainErrors.Person.CreditLimitCannotBeLessThanCurrentNegativeBalance);

        CreditLimit = creditLimit;
    }

    private void SetOpeningDate(DateOnly openingDate)
    {
        if (openingDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException(DomainErrors.Person.OpeningDateCannotBeInFuture);

        OpeningDate = openingDate;
    }

    // Constructor keeps calling this — no history yet, so overwrite is correct.
    private void SetInitialBalance(decimal initialBalance)
    {
        if (initialBalance < CreditLimit.GetValueOrDefault(0) * -1)
            throw new DomainException(DomainErrors.Person.InitialBalanceCannotBeLessThanCreditLimit);

        InitialBalance = initialBalance;
        CurrentBalance = initialBalance;
    }

    // UpdateDetails calls this instead — preserves everything AdjustBalance has accrued.
    private void UpdateInitialBalance(decimal newInitialBalance)
    {
        var delta = newInitialBalance - InitialBalance;
        InitialBalance = newInitialBalance;
        CurrentBalance += delta;
    }

    public bool CanWithdraw(decimal amount)
    {
        if (CreditLimit.HasValue)
            return amount <= CurrentBalance + CreditLimit.GetValueOrDefault(0);
        else
            return true;
    }

    public void AdjustBalance(decimal amount)
    {
        decimal newBalance = CurrentBalance + amount;
        if (CreditLimit.HasValue && newBalance < CreditLimit.GetValueOrDefault(0) * -1) // Ensure current balance does not go below negative credit limit
            throw new DomainException(DomainErrors.Person.CurrentBalanceCannotBeLessThanCreditLimit);

        CurrentBalance = newBalance;
    }


}
