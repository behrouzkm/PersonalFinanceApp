using System.Reflection.Metadata;
using PersonalFinanceApp.Domain.Errors;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Domain.Entities;

public class Currency : IReorderable
{
    public int Id { get; private set; }

    public string Code { get; private set; } = string.Empty!;

    public string Name { get; private set; } = string.Empty!;

    public bool IsActive { get; private set; }

    public int DisplayOrder { get; private set; }

    public byte DecimalPlaces { get; private set; } = 2;

    public string Symbol { get; private set; } = string.Empty!;




    private Currency() { }

    public Currency(string code, string name, bool isActive, int displayOrder, byte decimalPlaces = 2, string symbol = "")
    {
        ChangeCode(code);
        ChangeName(name);
        IsActive = isActive;
        SetDisplayOrder(displayOrder);
        SetDecimalPlaces(decimalPlaces);
        ChangeSymbol(symbol);
    }

    public void UpdateCurrency(string code, string name, bool isActive, byte decimalPlaces, string symbol)
    {
        ChangeCode(code);
        ChangeName(name);
        IsActive = isActive;
        SetDecimalPlaces(decimalPlaces);
        ChangeSymbol(symbol);
    }


    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(DomainErrors.Currency.NameRequired);

        Name = name.Trim();
    }

    public void ChangeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException(DomainErrors.Currency.CodeRequired);

        if (code.Length != 3 || !code.All(c => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z'))
            throw new DomainException(DomainErrors.Currency.CodeMustBeThreeLetters);

        Code = code.Trim().ToUpperInvariant();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public void SetDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
            throw new DomainException(DomainErrors.Currency.DisplayOrderCannotBeNegative);

        DisplayOrder = displayOrder;
    }

    public void IncrementDisplayOrder() => DisplayOrder++;

    public void DecrementDisplayOrder()
    {
        if (DisplayOrder > 0)
            DisplayOrder--;
    }


    public void SetDecimalPlaces(byte decimalPlaces)
    {
        if (decimalPlaces > 3)
            throw new DomainException(DomainErrors.Currency.DecimalPlacesTooHigh);

        DecimalPlaces = decimalPlaces;
    }

    public void ChangeSymbol(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new DomainException(DomainErrors.Currency.SymbolRequired);

        Symbol = symbol.Trim();
    }
}
