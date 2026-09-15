using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace PersonalFinanceApp.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;
    private readonly string _rootPathWithSeparator;

    public LocalFileStorageService(IConfiguration configuration)
    {
        _rootPath = Path.GetFullPath(
            configuration["Storage:LocalPath"]
            ?? Path.Combine(AppContext.BaseDirectory, "attachments"));

        _rootPathWithSeparator =
            _rootPath.TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
    }

    public async Task<string> SaveAsync(
        Stream content,
        string suggestedFileName,
        CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_rootPath);

        var storageKey =
            $"{Guid.NewGuid()}{Path.GetExtension(suggestedFileName)}";

        var filePath = GetSafeFilePath(storageKey);

        await using var fileStream = File.Create(filePath);
        await content.CopyToAsync(fileStream, cancellationToken);

        return storageKey;
    }

    public Task<Stream> OpenReadAsync(
        string storageKey,
        CancellationToken cancellationToken)
    {
        var filePath = GetSafeFilePath(storageKey);

        return Task.FromResult<Stream>(
            File.OpenRead(filePath));
    }

    public Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken)
    {
        var filePath = GetSafeFilePath(storageKey);

        File.Delete(filePath);

        return Task.CompletedTask;
    }

    private string GetSafeFilePath(string storageKey)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
            throw new ArgumentException(
                "Storage key is required.",
                nameof(storageKey));

        var fullPath = Path.GetFullPath(
            Path.Combine(_rootPath, storageKey));

        if (!fullPath.StartsWith(
                _rootPathWithSeparator,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Invalid storage key.");
        }

        return fullPath;
    }
}
