using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Constants;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Features.Currencies.Common;
using PersonalFinanceApp.Application.Features.Languages.Common;
using PersonalFinanceApp.Application.Features.Profile.Common;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.Profile.Queries.GetMyProfile;

public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, MyProfileDto>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyProfileQueryHandler(
        IIdentityService identityService, IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _identityService = identityService;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<MyProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        // Everything about the ApplicationUser goes through IIdentityService -
        // Application never queries the Identity tables directly (they're not
        // even exposed on IApplicationDbContext).
        var profile = await _identityService.GetProfileAsync(_currentUser.UserId, cancellationToken);
        LanguageOptionDto? userLanguage = null;

        var tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Id == _currentUser.TenantId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tenant), _currentUser.TenantId);

        var tenantLanguage = await _context.Languages
                .Where(l => l.Id == tenant.DefaultLanguageId)
                .Select(l => new LanguageOptionDto { Id = l.Id, Code = l.Code, Name = l.Name })
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Language), tenant.DefaultLanguageId);

        if(profile.LanguageId.HasValue)
        {

            userLanguage = await _context.Languages
                    .Where(l => l.Id == profile.LanguageId.Value)
                    .Select(l => new LanguageOptionDto { Id = l.Id, Code = l.Code, Name = l.Name })
                    .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException(nameof(Language), profile.LanguageId.Value);
        }
        var tenantCurrency = await _context.Currencies
                .Where(c => c.Id == tenant.DefaultCurrencyId)
                .Select(c => new CurrencyOptionDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    Symbol = c.Symbol,
                    DecimalPlaces = c.DecimalPlaces
                })
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Currency), tenant.DefaultCurrencyId);

        return new MyProfileDto
        {
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Email = profile.Email,
            ProfilePhotoStorageKey = profile.ProfilePhotoStorageKey,
            DateOfBirth = profile.DateOfBirth,
            Gender = profile.Gender,
            UserLanguage = userLanguage,
            CreatedAtUtc = profile.CreatedAtUtc,
            LastLoginAtUtc = profile.LastLoginAtUtc,
            PasswordChangedAtUtc = profile.PasswordChangedAtUtc,
            TenantLanguage = tenantLanguage,
            TenantCurrency = tenantCurrency,
            CanEditTenantSettings = _currentUser.IsInRole(Roles.TenantAdministrators)
        };
    }
}
