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

public class Person : BaseAuditableEntity, IFundSource
{


    public PersonType PersonType { get; private set; } = PersonType.Individual;

    public string DisplayName { get; private set; } = string.Empty!;


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

    public string? Email { get; private set; }

    public string? MobileNumber { get; private set; }

    public string? TelNumber { get; private set; }


    private Person() { }

    public Person(PersonType personType, string displayName, Guid ledgerAccountId, byte currencyId,
                    decimal initialBalance, decimal creditLimit, int displayOrder, string? email, string? mobileNumber,
                    string? telNumber, Guid tenantId, Guid createdBy, string? description = null) :
                            base(tenantId, createdBy, description)
    {
        SetPersonType(personType);
        SetDisplayName(displayName);
        SetLedgerAccountId(ledgerAccountId);
        SetCurrencyId(currencyId);
        SetInitialBalance(initialBalance);
        SetCreditLimit(creditLimit);
        SetDisplayOrder(displayOrder);
        SetEmail(email);
        SetMobileNumber(mobileNumber);
        SetTelNumber(telNumber);
    }


    public void UpdatePerson(PersonType personType, string displayName, Guid ledgerAccountId, byte currencyId,
                                decimal initialBalance, decimal creditLimit, int displayOrder, string? email,
                                string? mobileNumber, string? telNumber, string? description = null)
    {
        SetPersonType(personType);
        SetDisplayName(displayName);
        SetLedgerAccountId(ledgerAccountId);
        SetCurrencyId(currencyId);
        SetInitialBalance(initialBalance);
        SetCreditLimit(creditLimit);
        SetDisplayOrder(displayOrder);
        SetEmail(email);
        SetMobileNumber(mobileNumber);
        SetTelNumber(telNumber);

        SetDescription(description);
    }

    public void SetPersonType(PersonType personType)
    {
        PersonType = personType;
    }


    public void SetDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new DomainException(DomainErrors.Person.DisplayNameRequired);

        DisplayName = displayName.Trim();
    }


    public void SetEmail(string? email)
    {
        if (!string.IsNullOrWhiteSpace(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new DomainException(DomainErrors.Person.InvalidEmailFormat);

        Email = email?.Trim().ToLower();
    }

    public void SetMobileNumber(string? mobileNumber)
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

    public void SetTelNumber(string? telNumber)
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

    public void SetLedgerAccountId(Guid ledgerAccountId)
    {
        if (ledgerAccountId == Guid.Empty)
            throw new DomainException(DomainErrors.Person.LedgerAccountRequired);

        LedgerAccountId = ledgerAccountId;
    }

    public void SetCurrencyId(byte currencyId)
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

    public void SetCreditLimit(decimal? creditLimit)
    {
        if (creditLimit.HasValue && creditLimit.Value < 0)
            throw new DomainException(DomainErrors.Person.CreditLimitCannotBeNegative);

        if (CurrentBalance < 0 && creditLimit.HasValue && creditLimit.Value < Math.Abs(CurrentBalance))
            throw new DomainException(DomainErrors.Person.CreditLimitCannotBeLessThanCurrentNegativeBalance);

        CreditLimit = creditLimit;
    }

    public void SetInitialBalance(decimal initialBalance)
    {

        if (initialBalance < CreditLimit.GetValueOrDefault(0) * -1) // Ensure initial balance is not less than negative credit limit
            throw new DomainException(DomainErrors.Person.InitialBalanceCannotBeLessThanCreditLimit);

        InitialBalance = initialBalance;
        CurrentBalance = initialBalance; // Set current balance to initial balance when creating the person
    }


    public bool CanWithdraw(decimal amount)
    {
        if(CreditLimit.HasValue)
            return amount <= CurrentBalance + CreditLimit.GetValueOrDefault(0);
        else
            return true;
    }

    public void AdjustBalance(decimal amount)
    {
        CurrentBalance += amount;
    }


}
