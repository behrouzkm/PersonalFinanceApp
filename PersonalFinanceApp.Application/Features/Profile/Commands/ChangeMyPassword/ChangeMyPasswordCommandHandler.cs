using MediatR;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;

namespace PersonalFinanceApp.Application.Features.Profile.Commands.ChangeMyPassword;

public class ChangeMyPasswordCommandHandler : IRequestHandler<ChangeMyPasswordCommand>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUser;

    public ChangeMyPasswordCommandHandler(IIdentityService identityService, ICurrentUserService currentUser)
    {
        _identityService = identityService;
        _currentUser = currentUser;
    }

    public async Task Handle(ChangeMyPasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.ChangePasswordAsync(
            _currentUser.UserId, request.CurrentPassword, request.NewPassword, cancellationToken);

        if (!result.Succeeded)
            throw new BusinessRuleException(ApplicationErrorCodes.Profile.ChangePasswordFailed, result.Errors);
    }
}
