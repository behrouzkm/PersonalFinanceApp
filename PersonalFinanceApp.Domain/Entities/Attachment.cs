using PersonalFinanceApp.Domain.Common;
using PersonalFinanceApp.Domain.Enums;
using PersonalFinanceApp.Domain.Errors;

public class Attachment : BaseAuditableEntity
{
    public Guid ReferenceId { get; private set; }
    public AttachmentReferenceType ReferenceType { get; private set; }

    public string FileName { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long FileSize { get; private set; }


    private Attachment() { }


    public Attachment(AttachmentReferenceType referenceType, string fileName, string filePath,
                        string contentType, long fileSize, Guid tenantId, Guid createdBy, string? description = null)
                        : base(tenantId, createdBy, description)
    {
        SetReferenceType(referenceType);
        SetFileName(fileName);
        SetFilePath(filePath);
        SetContentType(contentType);
        SetFileSize(fileSize);
    }

    public void SetReferenceType(AttachmentReferenceType referenceType)
    {
        ReferenceType = referenceType;
    }

    public void SetFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(DomainErrors.Attachment.FileNameRequired);

        FileName = name.Trim();
    }
    public void SetFilePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new DomainException(DomainErrors.Attachment.FilePathRequired);

        FilePath = path.Trim();
    }

    public void SetContentType(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            throw new DomainException(DomainErrors.Attachment.FileContentRequired);

        ContentType = contentType.Trim();
    }

    public void SetFileSize(long size)
    {
        if (size <= 0)
            throw new DomainException(DomainErrors.Attachment.FileContentRequired);

        FileSize = size;
    }

}
