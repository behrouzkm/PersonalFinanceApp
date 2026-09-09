namespace PersonalFinanceApp.Application.Common.Interfaces;

// Storage mechanism is swappable independent of everything above — local disk in
// dev, Blob/S3 in production — same reasoning as IIdentityService keeping Identity
// framework details out of Application.
public interface IFileStorageService
{
    Task<string> SaveAsync(Stream content, string suggestedFileName, CancellationToken cancellationToken);
    Task<Stream> OpenReadAsync(string storageKey, CancellationToken cancellationToken);
    Task DeleteAsync(string storageKey, CancellationToken cancellationToken);
}
