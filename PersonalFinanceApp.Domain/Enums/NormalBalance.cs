using System.ComponentModel;

namespace PersonalFinanceApp.Domain.Enums;

public enum NormalBalance : byte
{
    Debit = 1,
    Credit = 2,
    FloatingBalance = 3
}
