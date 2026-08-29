namespace PersonalFinanceApp.Domain.Enums;

// Standard Enum for database entities
public enum AccountCategory : byte
{
    ExpenseAccount = 1,
    IncomeAccount = 2,
    PersonAccount = 3,
    BankAccount = 4,
    CashAccount = 5,
    OpeningBalanceEquity = 6
}
