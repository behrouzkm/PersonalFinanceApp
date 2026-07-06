using System.ComponentModel.DataAnnotations;
using PersonalFinanceApp.Domain.Enums;
using PersonalFinanceApp.Domain.Errors;

namespace PersonalFinanceApp.Domain.Entities;

public class DocumentTypeTranslation
{
    public int Id { get; set; }

    // Foreign key to the related document type
    //public byte DocumentTypeId { get; private set; }
    public DocumentType DocumentType { get; private set; }


    // Foreign key to the related language
    public byte LanguageId { get; private set; }
    public Language Language { get; private set; } = null!;


    public string Name { get; private set; } = string.Empty!;
    public string? Description { get; private set; }


    private DocumentTypeTranslation() { }

    public DocumentTypeTranslation(DocumentType documentType, byte languageId, string name, string? description = null)
    {
        SetDocumentType(documentType);
        SetLanguage(languageId);
        ChangeName(name);
        SetDescription(description);
    }

    public void UpdateDocumentTypeTranslation(DocumentType documentType, byte languageId, string name, string? description = null)
    {
        SetDocumentType(documentType);
        SetLanguage(languageId);
        ChangeName(name);
        SetDescription(description);
    }

    public void SetLanguage(byte languageId) => LanguageId = languageId;

    public void SetDocumentType(DocumentType documentType) => DocumentType = documentType;

    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(DomainErrors.DocumentType.NameRequired);

        Name = name.Trim();
    }

    public void SetDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }


}
