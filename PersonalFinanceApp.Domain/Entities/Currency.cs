using PersonalFinanceApp.Domain.Errors;

namespace PersonalFinanceApp.Domain.Entities;

public class Currency
{
    public byte Id { get; private set; }

    public string Code { get; private set; } = string.Empty!;

    public string Name { get; private set; } = string.Empty!;

    public int DecimalPlaces { get; private set; } = 2;

    public string Symbol { get; private set; } = string.Empty!;


    private Currency() { }

    public Currency(string code, string name, int decimalPlaces = 2, string symbol = "")
    {
        ChangeCode(code);
        ChangeName(name);
        SetDecimalPlaces(decimalPlaces);
        ChangeSymbol(symbol);
    }

    public void UpdateCurrency(string code, string name, int decimalPlaces, string symbol)
    {
        ChangeCode(code);
        ChangeName(name);
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

        if(code.Length != 3 || !code.All(char.IsLetter))
            throw new DomainException(DomainErrors.Currency.CodeMustBeThreeLetters);

        Code = code.Trim().ToUpperInvariant();
    }

    public void SetDecimalPlaces(int decimalPlaces)
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
