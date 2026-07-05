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

    public BankAccount(string name, Guid ledgerAccountId, byte currencyId, decimal initialBalance, int displayOrder,
                            Guid tenantId, Guid createdBy, string bankName, string branchName, BankAccountType bankAccountType,
                            string bankAccountNumber, string? iban = null) :
                                base(name, ledgerAccountId, currencyId, initialBalance, displayOrder, tenantId, createdBy)
    {
        SetBankName(bankName);
        SetBranchName(branchName);
        BankAccountType = bankAccountType;
        SetBankAccountNumber(bankAccountNumber);
        SetIban(iban);
    }

    public void UpdateBankAccount(string bankName, string branchName, string bankAccountNumber, string? iban = null, BankAccountType? bankAccountType = null)
    {
        SetBankName(bankName);
        SetBranchName(branchName);
        SetBankAccountNumber(bankAccountNumber);
        SetIban(iban);
        if (bankAccountType.HasValue) BankAccountType = bankAccountType.Value;
    }

    public void SetBankName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(DomainErrors.BankAccount.BankNameRequired);

        BankName = name.Trim();
    }

    public void SetBranchName(string name)
    {
        BranchName = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
    }

    public void SetBankAccountNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new DomainException(DomainErrors.BankAccount.BankAccountNumberRequired);

        BankAccountNumber = number.Trim();
    }

    public void SetIban(string? iban)
    {
        IBAN = string.IsNullOrWhiteSpace(iban) ? null : iban.Trim();
    }
}
