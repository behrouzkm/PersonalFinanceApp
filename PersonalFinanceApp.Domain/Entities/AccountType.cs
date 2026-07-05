using System.ComponentModel.DataAnnotations;
using PersonalFinanceApp.Domain.Common;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Domain.Entities;

// Represents a type of account (e.g.,Bank/Wallet, Persons, Revenue/Income, Expense/Cost, Equity/Capital)
public class AccountType
{

    public byte Id { get; private set; }

    public bool IsActive { get; private set; }

    public AccountCategory Category { get; private set; }

    public NormalBalance NormalBalance { get; private set; }


    private AccountType() { }

    public AccountType(bool isActive, AccountCategory category, NormalBalance normalBalance)
    {
        IsActive = isActive;
        SetCategory(category);
        SetNormalBalance(normalBalance);
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public void SetNormalBalance(NormalBalance normalBalance)
    {
        NormalBalance = normalBalance;
    }

    public void SetCategory(AccountCategory category)
    {
        Category = category;
    }

}
