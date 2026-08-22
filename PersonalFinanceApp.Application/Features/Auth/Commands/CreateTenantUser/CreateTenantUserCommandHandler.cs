using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.Auth.Commands.CreateTenantUser;

public class CreateTenantUserCommandHandler : IRequestHandler<CreateTenantUserCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;

    public CreateTenantUserCommandHandler(
                IApplicationDbContext context,
                ICurrentUserService currentUser,
                IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
    }

    public async Task<Guid> Handle(CreateTenantUserCommand request, CancellationToken cancellationToken)
    {
        // tenant id comes from the authenticated caller's own claims (never from the request)
        // this can prevent for creating a user under a tenant other than their own
        var tenantId = _currentUser.TenantId;

        var tenant = await _context.Tenants
                        .FirstOrDefaultAsync(r => r.Id == tenantId, cancellationToken)
                        ?? throw new NotFoundException(nameof(Tenant), tenantId);

        if (!tenant.IsActive)
            throw new BusinessRuleException(ApplicationErrorCodes.Auth.TenantInactive, tenant.Id);

        var result = await _identityService.CreateUserForExistingTenantAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            tenantId,
            cancellationToken
        );

        if (!result.Succeeded)
            throw new BusinessRuleException(ApplicationErrorCodes.Auth.RegistrationFailed, result.Errors);

        return result.UserId!.Value;
    }
}
