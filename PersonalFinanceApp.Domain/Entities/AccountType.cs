using System.ComponentModel.DataAnnotations;
using PersonalFinanceApp.Domain.Common;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Domain.Entities;

// Represents a type of account (e.g.,Bank/Wallet, Persons, Revenue/Income, Expense/Cost, Equity/Capital)
// It's going to be seed when the project start, no-one even admins must not remove or change the data
public class AccountType
{

    public int Id { get; private set; }

    public AccountCategory Category { get; private set; }

    public NormalBalance NormalBalance { get; private set; }

    // Indicates whether the LedgerAccount can be deleted directly,
    // without first deleting an associated domain entity.
    public bool CanDeleteDirectly { get; private set; }

    private AccountType() { }

    public AccountType(AccountCategory category, NormalBalance normalBalance, bool canDeleteDirectly)
    {
        NormalBalance = normalBalance;
        Category = category;
        CanDeleteDirectly = canDeleteDirectly;
    }


}
