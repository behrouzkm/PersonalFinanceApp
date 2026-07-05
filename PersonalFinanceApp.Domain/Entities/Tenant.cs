using System.ComponentModel.DataAnnotations;
using PersonalFinanceApp.Domain.Errors;

namespace PersonalFinanceApp.Domain.Entities;

public class Tenant
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty!;

    public byte DefaultLanguageId { get; private set; }

    public byte DefaultCurrencyId { get; private set; }

    public bool IsActive { get; private set; }


    private Tenant() { }

    public Tenant(string name, byte defaultLanguageId, byte defaultCurrencyId, bool isActive = true)
    {
        ChangeName(name);
        SetDefaultLanguage(defaultLanguageId);
        SetDefaultCurrency(defaultCurrencyId);
        IsActive = isActive;
    }

    public void ChangeName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new DomainException(DomainErrors.Tenant.NameRequired);

        Name = newName.Trim();
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;

    public void SetDefaultLanguage(byte defaultLanguageId) => DefaultLanguageId = defaultLanguageId;

    public void SetDefaultCurrency(byte defaultCurrencyId) => DefaultCurrencyId = defaultCurrencyId;
}
