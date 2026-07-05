namespace PersonalFinanceApp.Domain.Enums;

[Flags]
public enum DocumentType
{
    None = 0,
    Expenditure = 1 << 0,  // 1
    Income = 1 << 1,   // 2
    Person = 1 << 2,   // 4
    MoneyTransfer = 1 << 3  // 8
}
