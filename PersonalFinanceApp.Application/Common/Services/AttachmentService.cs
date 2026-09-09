using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Features.Attachments.Common;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Common.Services;

public class AttachmentService : IAttachmentService
{

    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly ICurrentUserService _currentUser;

    public AttachmentService(IApplicationDbContext context, IFileStorageService fileStorage, ICurrentUserService currentUser)
    {
        _context = context;
        _fileStorage = fileStorage;
        _currentUser = currentUser;
    }

    public async Task<Guid> UploadAsync(AttachmentOwnerType ownerType, Guid ownerId, Stream content,
        string fileName, string contentType, long fileSizeBytes, CancellationToken cancellationToken)
    {
        // The one check the polymorphic-FK design doesn't get for free at upload
        // time: the target row has to exist (and, via the global query filter,
        // belong to the current tenant) before EF will even let the FK be set.
        await EnsureOwnerExistsAsync(ownerType, ownerId, cancellationToken);

        var storageKey = await _fileStorage.SaveAsync(content, fileName, cancellationToken);

        var attachment = ownerType switch
        {
            AttachmentOwnerType.AccountingDocument => Attachment.ForAccountingDocument(
                ownerId, fileName, contentType, fileSizeBytes, storageKey, _currentUser.TenantId, _currentUser.UserId),
            AttachmentOwnerType.Person => Attachment.ForPerson(
                ownerId, fileName, contentType, fileSizeBytes, storageKey, _currentUser.TenantId, _currentUser.UserId),
            AttachmentOwnerType.MonetaryAccount => Attachment.ForMonetaryAccount(
                ownerId, fileName, contentType, fileSizeBytes, storageKey, _currentUser.TenantId, _currentUser.UserId),
            AttachmentOwnerType.CurrencyExchange => Attachment.ForCurrencyExchange(
                ownerId, fileName, contentType, fileSizeBytes, storageKey, _currentUser.TenantId, _currentUser.UserId),
            _ => throw new ArgumentOutOfRangeException(nameof(ownerType))
        };

        _context.Attachments.Add(attachment);
        await _context.SaveChangesAsync(cancellationToken);
        return attachment.Id;
    }

    public async Task<(Stream, string, string)> DownloadAsync(Guid attachmentId, CancellationToken cancellationToken)
    {
        var attachment = await _context.Attachments
            .FirstOrDefaultAsync(a => a.Id == attachmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Attachment), attachmentId);

        var stream = await _fileStorage.OpenReadAsync(attachment.StorageKey, cancellationToken);
        return (stream, attachment.ContentType, attachment.FileName);
    }
    
    public async Task<IReadOnlyList<AttachmentDto>> GetForOwnerAsync(
        AttachmentOwnerType ownerType, Guid ownerId, CancellationToken cancellationToken)
    {
        var query = OwnerFilter(ownerType, ownerId);

        return await query
            .Select(a => new AttachmentDto
            {
                Id = a.Id,
                FileName = a.FileName,
                ContentType = a.ContentType,
                FileSizeBytes = a.FileSizeBytes,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid attachmentId, CancellationToken cancellationToken)
    {
        var attachment = await _context.Attachments.FirstOrDefaultAsync(a => a.Id == attachmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Attachment), attachmentId);

        attachment.SoftDelete(_currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);
        // Deliberately not deleting the underlying blob here — soft-deleted
        // attachments should stay recoverable via Restore, same as everything else.
    }

    public async Task SoftDeleteAllForOwnerAsync(
        AttachmentOwnerType ownerType, Guid ownerId, CancellationToken cancellationToken)
    {
        var attachments = await OwnerFilter(ownerType, ownerId).ToListAsync(cancellationToken);
        foreach (var attachment in attachments)
            attachment.SoftDelete(_currentUser.UserId);
        // Caller's own SaveChangesAsync persists this — same convention as
        // ReorderService.CloseGapAsync.
    }

    public async Task RestoreAllForOwnerAsync(
        AttachmentOwnerType ownerType, Guid ownerId, CancellationToken cancellationToken)
    {
        var attachments = await _context.Attachments.IgnoreQueryFilters()
            .Where(OwnerPredicate(ownerType, ownerId))
            .Where(a => a.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var attachment in attachments)
            attachment.Restore(_currentUser.UserId);
    }

    private IQueryable<Attachment> OwnerFilter(AttachmentOwnerType ownerType, Guid ownerId) =>
        _context.Attachments.Where(OwnerPredicate(ownerType, ownerId));

    private static Expression<Func<Attachment, bool>> OwnerPredicate(AttachmentOwnerType ownerType, Guid ownerId) =>
        ownerType switch
        {
            AttachmentOwnerType.AccountingDocument => a => a.AccountingDocumentId == ownerId,
            AttachmentOwnerType.Person => a => a.PersonId == ownerId,
            AttachmentOwnerType.MonetaryAccount => a => a.MonetaryAccountId == ownerId,
            AttachmentOwnerType.CurrencyExchange => a => a.CurrencyExchangeId == ownerId,
            _ => throw new ArgumentOutOfRangeException(nameof(ownerType))
        };

    private async Task EnsureOwnerExistsAsync(AttachmentOwnerType ownerType, Guid ownerId, CancellationToken cancellationToken)
    {
        var exists = ownerType switch
        {
            AttachmentOwnerType.AccountingDocument => await _context.AccountingDocuments.AnyAsync(d => d.Id == ownerId, cancellationToken),
            AttachmentOwnerType.Person => await _context.Persons.AnyAsync(p => p.Id == ownerId, cancellationToken),
            AttachmentOwnerType.MonetaryAccount => await _context.MonetaryAccounts.AnyAsync(m => m.Id == ownerId, cancellationToken),
            AttachmentOwnerType.CurrencyExchange => await _context.CurrencyExchanges.AnyAsync(e => e.Id == ownerId, cancellationToken),
            _ => false
        };

        if (!exists)
            throw new NotFoundException(ownerType.ToString(), ownerId);
    }
}
