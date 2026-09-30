using MediatR;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;

namespace PersonalFinanceApp.Application.Features.Profile.Commands.UpdateMyProfile;

public class UpdateMyProfileCommandHandler : IRequestHandler<UpdateMyProfileCommand>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUser;

    public UpdateMyProfileCommandHandler(IIdentityService identityService, ICurrentUserService currentUser)
    {
        _identityService = identityService;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.UpdateProfileAsync(
            _currentUser.UserId, request.FirstName, request.LastName, request.DateOfBirth, request.Gender,
            cancellationToken);

        if (!result.Succeeded)
            throw new BusinessRuleException(ApplicationErrorCodes.Profile.UpdateFailed, result.Errors);
    }
}
