using System;
using PersonalFinanceApp.Domain.Enums;
using PersonalFinanceApp.Domain.Errors;

namespace PersonalFinanceApp.Domain.Entities;

public class BankAccount : MonetaryAccount
{
    public string BankName { get; private set; } = string.Empty!;

    public string? BranchName { get; private set; }

    public BankAccountType BankAccountType { get; private set; }

    public string BankAccountNumber { get; private set; } = string.Empty!;

    public string? IBAN { get; private set; }

    private BankAccount() { }

    public BankAccount(
        string displayName,
        Guid ledgerAccountId,
        int currencyId,
        DateOnly openingDate,
        decimal initialBalance,
        int displayOrder,
        Guid tenantId,
        Guid createdBy,
        string bankName,
        string? branchName,
        BankAccountType bankAccountType,
        string bankAccountNumber,
        string? iban = null,
        string? description = null,
        decimal creditLimit = 0,
        Guid? openingAccountingDocumentId = null) :
                        base(displayName, ledgerAccountId, currencyId, openingDate, initialBalance,
                            displayOrder, tenantId, createdBy, creditLimit, description, openingAccountingDocumentId)
    {
        SetBankName(bankName);
        SetBranchName(branchName);
        BankAccountType = bankAccountType;
        SetBankAccountNumber(bankAccountNumber);
        SetIban(iban);
    }

    public void UpdateBankAccount(
       string displayName,
        int currencyId,
        DateOnly openingDate,
        decimal initialBalance,
         string bankName,
        string? branchName,
        string bankAccountNumber,
         Guid modifiedBy,
        decimal creditLimit,
        string? iban = null,
        string? description = null,
        BankAccountType? bankAccountType = null)
    {

        SetBankName(bankName);
        SetBranchName(branchName);
        SetBankAccountNumber(bankAccountNumber);
        SetIban(iban);
        if (bankAccountType.HasValue)
            BankAccountType = bankAccountType.Value;

        UpdateMonetaryAccount(
            displayName,
            currencyId,
            openingDate,
            initialBalance,
            modifiedBy,
            creditLimit,
            description);
    }

    private void SetBankName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(DomainErrors.BankAccount.BankNameRequired);

        BankName = name.Trim();
    }

    private void SetBranchName(string? name)
    {
        BranchName = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
    }

    private void SetBankAccountNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new DomainException(DomainErrors.BankAccount.BankAccountNumberRequired);

        BankAccountNumber = number.Trim();
    }

    private void SetIban(string? iban)
    {
        IBAN = string.IsNullOrWhiteSpace(iban) ? null : iban.Trim();
    }
}
