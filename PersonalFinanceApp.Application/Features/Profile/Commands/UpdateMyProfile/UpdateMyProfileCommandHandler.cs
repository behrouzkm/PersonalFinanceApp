using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.Profile.Commands.UpdateMyProfile;

public class UpdateMyProfileCommandHandler : IRequestHandler<UpdateMyProfileCommand>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUser;
    private readonly IApplicationDbContext _context;

    public UpdateMyProfileCommandHandler(
        IIdentityService identityService,
        ICurrentUserService currentUser,
        IApplicationDbContext context)
    {
        _identityService = identityService;
        _currentUser = currentUser;
        _context=context;
    }

    public async Task Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
    {
        if (request.LanguageId is not null)
        {
            var language = await _context.Languages
                    .FirstOrDefaultAsync(l => l.Id == request.LanguageId, cancellationToken)
                ?? throw new NotFoundException(nameof(Language), request.LanguageId);

            if (!language.IsActive)
                throw new BusinessRuleException(ApplicationErrorCodes.Language.LanguageDeactivated, request.LanguageId);
        }

        var result = await _identityService.UpdateProfileAsync(
            _currentUser.UserId, request.FirstName, request.LastName, request.DateOfBirth, request.Gender,
            request.LanguageId, cancellationToken);

        if (!result.Succeeded)
            throw new BusinessRuleException(ApplicationErrorCodes.Profile.UpdateFailed, result.Errors);
    }
}

