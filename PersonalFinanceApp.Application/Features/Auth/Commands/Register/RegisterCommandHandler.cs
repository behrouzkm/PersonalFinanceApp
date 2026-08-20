using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;

namespace PersonalFinanceApp.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
{

    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.RegisterAsync(
            request.Email,
            request.Password,
            request.TenantName,
            request.FirstName,
            request.LastName,
            request.DefaultLanguageId,
            request.DefaultCurrencyId,
            cancellationToken
        );

        if (!result.Succeeded)
        {
            throw new BusinessRuleException(ApplicationErrorCodes.Auth.RegistrationFailed, result.Errors);
        }

        return result.UserId!.Value;
    }
}
