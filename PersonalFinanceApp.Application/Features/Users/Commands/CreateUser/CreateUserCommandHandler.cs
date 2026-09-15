using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;

namespace PersonalFinanceApp.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUser;

    public CreateUserCommandHandler(IIdentityService identityService, ICurrentUserService currentUser)
    {
        _identityService = identityService;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.CreateUserForExistingTenantAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            _currentUser.TenantId,
            cancellationToken);

        if (!result.Succeeded)
            throw new BusinessRuleException(ApplicationErrorCodes.Auth.RegistrationFailed, result.Errors);

        return result.UserId!.Value;
    }
}
