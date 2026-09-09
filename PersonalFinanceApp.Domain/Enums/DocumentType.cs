namespace PersonalFinanceApp.Domain.Enums;

[Flags]
public enum DocumentType
{
    None = 0,
    Expenditure = 1 << 0,  // 1
    Income = 1 << 1,   // 2
    MoneyTransfer = 1 << 2,  // 4
    OpeningBalance = 1 << 3,  // 8
    CurrencyExchange = 1 << 4  // 16
}
