using System.ComponentModel.DataAnnotations;
using PersonalFinanceApp.Domain.Errors;

namespace PersonalFinanceApp.Domain.Entities;

public class Tenant
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty!;

    public int DefaultLanguageId { get; private set; }

    public int DefaultCurrencyId { get; private set; }

    public bool IsActive { get; private set; }


    private Tenant() { }

    public Tenant(string name, int defaultLanguageId, int defaultCurrencyId, bool isActive = true)
    {
        SetName(name);
        SetDefaultLanguage(defaultLanguageId);
        SetDefaultCurrency(defaultCurrencyId);
        IsActive = isActive;
    }

    private void SetName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new DomainException(DomainErrors.Tenant.NameRequired);

        Name = newName.Trim();
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;

    private void SetDefaultLanguage(int defaultLanguageId) => DefaultLanguageId = defaultLanguageId;

    private void SetDefaultCurrency(int defaultCurrencyId) => DefaultCurrencyId = defaultCurrencyId;
}
