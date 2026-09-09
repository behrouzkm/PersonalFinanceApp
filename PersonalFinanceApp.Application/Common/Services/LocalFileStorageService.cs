using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace PersonalFinanceApp.Application.Common.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;

    public LocalFileStorageService(IConfiguration configuration) =>
        _rootPath = configuration["Storage:LocalPath"] ?? Path.Combine(AppContext.BaseDirectory, "attachments");

    public async Task<string> SaveAsync(Stream content, string suggestedFileName, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_rootPath);
        var storageKey = $"{Guid.NewGuid()}{Path.GetExtension(suggestedFileName)}";
        await using var fileStream = File.Create(Path.Combine(_rootPath, storageKey));
        await content.CopyToAsync(fileStream, cancellationToken);
        return storageKey;
    }

    public Task<Stream> OpenReadAsync(string storageKey, CancellationToken cancellationToken) =>
        Task.FromResult<Stream>(File.OpenRead(Path.Combine(_rootPath, storageKey)));

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken)
    {
        File.Delete(Path.Combine(_rootPath, storageKey));
        return Task.CompletedTask;
    }
}
