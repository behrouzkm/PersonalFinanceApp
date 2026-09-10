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


    public string Translation { get; private set; } = string.Empty!;
    public string? Description { get; private set; }

    private AccountTypeTranslation() { }

    public AccountTypeTranslation(int accountTypeId, int languageId, string translation, string? description = null)
    {
        SetAccountType(accountTypeId);
        SetLanguage(languageId);
        SetTranslation(translation);
        SetDescription(description);
    }


    public void UpdateAccountTypeTranslation(int accountTypeId, int languageId, string translation, string? description = null)
    {
        SetAccountType(accountTypeId);
        SetLanguage(languageId);
        SetTranslation(translation);
        SetDescription(description);
    }

    private void SetAccountType(int accountTypeId) => AccountTypeId = accountTypeId;

    private void SetLanguage(int languageId) => LanguageId = languageId;

    private void SetTranslation(string translation)
    {
        if (string.IsNullOrWhiteSpace(translation))
            throw new DomainException(DomainErrors.AccountTypeTranslation.TranslationRequired);

        Translation = translation.Trim();
    }

    private void SetDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}
