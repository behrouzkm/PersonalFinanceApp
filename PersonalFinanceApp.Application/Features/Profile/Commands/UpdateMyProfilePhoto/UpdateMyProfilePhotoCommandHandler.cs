using MediatR;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;

namespace PersonalFinanceApp.Application.Features.Profile.Commands.UpdateMyProfilePhoto;

public class UpdateMyProfilePhotoCommandHandler : IRequestHandler<UpdateMyProfilePhotoCommand>
{
    private readonly IIdentityService _identityService;
    private readonly IFileStorageService _fileStorage;
    private readonly IFileContentValidator _fileContentValidator;
    private readonly ICurrentUserService _currentUser;

    public UpdateMyProfilePhotoCommandHandler(
        IIdentityService identityService, IFileStorageService fileStorage,
        IFileContentValidator fileContentValidator, ICurrentUserService currentUser)
    {
        _identityService = identityService;
        _fileStorage = fileStorage;
        _fileContentValidator = fileContentValidator;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateMyProfilePhotoCommand request, CancellationToken cancellationToken)
    {
        // Same content check UploadAttachmentCommandHandler already runs -
        // reused directly, not reimplemented.
        if (!await _fileContentValidator.IsValidAsync(request.Content, request.ContentType, cancellationToken))
            throw new BusinessRuleException(ApplicationErrorCodes.Attachment.InvalidFileContent);

        var profile = await _identityService.GetProfileAsync(_currentUser.UserId, cancellationToken);
        var previousStorageKey = profile.ProfilePhotoStorageKey;

        var newStorageKey = await _fileStorage.SaveAsync(request.Content, request.FileName, cancellationToken);

        var result = await _identityService.SetProfilePhotoAsync(_currentUser.UserId, newStorageKey, cancellationToken);
        if (!result.Succeeded)
        {
            // Roll back the just-saved file rather than leaving an orphaned
            // blob if the DB update failed.
            await _fileStorage.DeleteAsync(newStorageKey, cancellationToken);
            throw new BusinessRuleException(ApplicationErrorCodes.Profile.UpdateFailed, result.Errors);
        }

        // Only delete the OLD file after the new one is confirmed saved.
        if (!string.IsNullOrWhiteSpace(previousStorageKey))
            await _fileStorage.DeleteAsync(previousStorageKey, cancellationToken);
    }
}
