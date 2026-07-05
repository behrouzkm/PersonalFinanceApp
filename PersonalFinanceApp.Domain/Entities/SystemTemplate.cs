using System.ComponentModel.DataAnnotations;
using PersonalFinanceApp.Domain.Errors;

namespace PersonalFinanceApp.Domain.Entities;

public class SystemTemplate
{
    public int Id { get; private set; }

    public string TemplateKey { get; private set; } = string.Empty!; // e.g., "DOCUMENT_TYPE_En", "DefaultAccounts_fr", etc.

    public string JsonData { get; private set; } = string.Empty!;


    // Foreign key to the related language
    public byte LanguageId { get; private set; }
    public Language Language { get; private set; } = null!;


    private SystemTemplate() { }

    public SystemTemplate(string templateKey, string jsonData, byte languageId)
    {
        SetTemplateKey(templateKey);
        SetJsonData(jsonData);
        SetLanguage(languageId);
    }

    public void UpdateSystemTemplate(string templateKey, string jsonData, byte languageId)
    {
        SetTemplateKey(templateKey);
        SetJsonData(jsonData);
        SetLanguage(languageId);
    }

    public void SetLanguage(byte languageId) => LanguageId = languageId;

    public void SetTemplateKey(string templateKey)
    {
        if (string.IsNullOrWhiteSpace(templateKey))
            throw new DomainException(DomainErrors.SystemTemplate.TemplateKeyRequired);

        TemplateKey = templateKey.Trim();
    }

    public void SetJsonData(string jsonData)
    {
        if (string.IsNullOrWhiteSpace(jsonData))
            throw new DomainException(DomainErrors.SystemTemplate.JsonDataRequired);

        JsonData = jsonData.Trim();
    }
}
