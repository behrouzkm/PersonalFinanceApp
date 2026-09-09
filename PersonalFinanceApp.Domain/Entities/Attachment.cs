using System.ComponentModel.DataAnnotations.Schema;
using PersonalFinanceApp.Domain.Common;
using PersonalFinanceApp.Domain.Enums;
using PersonalFinanceApp.Domain.Errors;

namespace PersonalFinanceApp.Domain.Entities;

public class Attachment : BaseAuditableEntity
{
    public Guid? AccountingDocumentId { get; private set; }
    public AccountingDocument? AccountingDocument { get; private set; }

    public Guid? PersonId { get; private set; }
    public Person? Person { get; private set; }

    public Guid? MonetaryAccountId { get; private set; }
    public MonetaryAccount? MonetaryAccount { get; private set; }

    public Guid? CurrencyExchangeId { get; private set; }
    public CurrencyExchange? CurrencyExchange { get; private set; }

    public string FileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long FileSizeBytes { get; private set; }
    public string StorageKey { get; private set; } = null!;

    // Derived, not persisted — convenience for DTO mapping and query branching,
    // computed from whichever FK is actually set rather than stored redundantly.
    [NotMapped]
    public AttachmentOwnerType OwnerType => this switch
    {
        { AccountingDocumentId: not null } => AttachmentOwnerType.AccountingDocument,
        { PersonId: not null } => AttachmentOwnerType.Person,
        { MonetaryAccountId: not null } => AttachmentOwnerType.MonetaryAccount,
        { CurrencyExchangeId: not null } => AttachmentOwnerType.CurrencyExchange,
        _ => throw new DomainException(DomainErrors.Attachment.NoOwnerAssigned)
    };


    private Attachment() { }


    private Attachment(string fileName, string contentType, long fileSizeBytes, string storageKey,
        Guid tenantId, Guid createdBy) : base(tenantId, createdBy)
    {
        SetFileName(fileName);
        SetFileSize(fileSizeBytes);
        SetContentType(contentType);

        StorageKey = storageKey;
    }

    public static Attachment ForAccountingDocument(Guid documentId, string fileName, string contentType,
        long fileSizeBytes, string storageKey, Guid tenantId, Guid createdBy) =>
        new(fileName, contentType, fileSizeBytes, storageKey, tenantId, createdBy) { AccountingDocumentId = documentId };

    public static Attachment ForPerson(Guid personId, string fileName, string contentType,
        long fileSizeBytes, string storageKey, Guid tenantId, Guid createdBy) =>
        new(fileName, contentType, fileSizeBytes, storageKey, tenantId, createdBy) { PersonId = personId };

    public static Attachment ForMonetaryAccount(Guid monetaryAccountId, string fileName, string contentType,
        long fileSizeBytes, string storageKey, Guid tenantId, Guid createdBy) =>
        new(fileName, contentType, fileSizeBytes, storageKey, tenantId, createdBy) { MonetaryAccountId = monetaryAccountId };

    public static Attachment ForCurrencyExchange(Guid currencyExchangeId, string fileName, string contentType,
        long fileSizeBytes, string storageKey, Guid tenantId, Guid createdBy) =>
        new(fileName, contentType, fileSizeBytes, storageKey, tenantId, createdBy) { CurrencyExchangeId = currencyExchangeId };


    private void SetFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(DomainErrors.Attachment.FileNameRequired);

        FileName = name.Trim();
    }

    private void SetContentType(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            throw new DomainException(DomainErrors.Attachment.FileContentRequired);

        ContentType = contentType.Trim();
    }

    private void SetFileSize(long fileSizeBytes)
    {
        if (fileSizeBytes <= 0)
            throw new DomainException(DomainErrors.Attachment.FileSizeMustBePositive);

        FileSizeBytes = fileSizeBytes;
    }

}
