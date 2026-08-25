using PersonalFinanceApp.Domain.Errors;

namespace PersonalFinanceApp.Domain.Entities;

public class Currency
{
    public byte Id { get; private set; }

    public string Code { get; private set; } = string.Empty!;

    public string Name { get; private set; } = string.Empty!;

    public byte DecimalPlaces { get; private set; } = 2;

    public string Symbol { get; private set; } = string.Empty!;


    private Currency() { }

    public Currency(byte id, string code, string name, byte decimalPlaces = 2, string symbol = "")
    {
        SetId(id);
        ChangeCode(code);
        ChangeName(name);
        SetDecimalPlaces(decimalPlaces);
        ChangeSymbol(symbol);
    }

    public void UpdateCurrency(string code, string name, byte decimalPlaces, string symbol)
    {
        ChangeCode(code);
        ChangeName(name);
        SetDecimalPlaces(decimalPlaces);
        ChangeSymbol(symbol);
    }

    private void SetId(byte id)
    {
        if (id == 0)
            throw new DomainException(DomainErrors.Currency.InvalidId);

        Id = id;
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

        if (code.Length != 3 || !code.All(c => c is 'A' and <= 'Z'))
            throw new DomainException(DomainErrors.Currency.CodeMustBeThreeLetters);

        Code = code.Trim().ToUpperInvariant();
    }

    public void SetDecimalPlaces(byte decimalPlaces)
    {
        if (decimalPlaces < 0)
            throw new DomainException(DomainErrors.Currency.DecimalPlacesInvalid);

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
