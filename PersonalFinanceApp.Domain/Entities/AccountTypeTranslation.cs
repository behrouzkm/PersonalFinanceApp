using System.ComponentModel.DataAnnotations;
using PersonalFinanceApp.Domain.Errors;

namespace PersonalFinanceApp.Domain.Entities;

public class AccountTypeTranslation
{
    public int Id { get; private set; }


    // Foreign key to the related account type
    public int AccountTypeId { get; private set; }
    public AccountType AccountType { get; private set; } = null!;


    // Foreign key to the related language
    public int LanguageId { get; private set; }
    public Language Language { get; private set; } = null!;


    public string Name { get; private set; } = string.Empty!;
    public string? Description { get; private set; }

    private AccountTypeTranslation() { }

    public AccountTypeTranslation(int accountTypeId, int languageId, string name, string? description = null)
    {
        SetAccountType(accountTypeId);
        SetLanguage(languageId);
        ChangeName(name);
        SetDescription(description);
    }


    public void UpdateAccountTypeTranslation(int accountTypeId, int languageId, string name, string? description = null)
    {
        SetAccountType(accountTypeId);
        SetLanguage(languageId);
        ChangeName(name);
        SetDescription(description);
    }

    public void SetAccountType(int accountTypeId) => AccountTypeId = accountTypeId;

    public void SetLanguage(int languageId) => LanguageId = languageId;

    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(DomainErrors.LedgerAccount.NameRequired);

        Name = name.Trim();
    }

    public void SetDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}
