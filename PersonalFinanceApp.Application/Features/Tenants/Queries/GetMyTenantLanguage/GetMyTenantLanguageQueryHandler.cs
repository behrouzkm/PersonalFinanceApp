using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Features.Languages.Common;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.Tenants.Queries.GetMyTenantLanguage;

public class GetMyTenantLanguageQueryHandler : IRequestHandler<GetMyTenantLanguageQuery, LanguageOptionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;

    public GetMyTenantLanguageQueryHandler(
        IApplicationDbContext context, ICurrentUserService currentUser, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
    }

    public async Task<LanguageOptionDto> Handle(GetMyTenantLanguageQuery request, CancellationToken cancellationToken)
    {
        var profile = await _identityService.GetProfileAsync(_currentUser.UserId, cancellationToken);

        int languageId;
        if (profile.LanguageId is not null)
        {
            // Personal preference wins when set.
            languageId = profile.LanguageId.Value;
        }
        else
        {
            // Falls back to the tenant's invite-time default - this is the
            // ONLY remaining purpose of Tenant.DefaultLanguageId now that
            // language is per-user: a sensible starting point for a newly
            // invited member who hasn't set a personal preference yet.
            var tenant = await _context.Tenants
                    .FirstOrDefaultAsync(t => t.Id == _currentUser.TenantId, cancellationToken)
                ?? throw new NotFoundException(nameof(Tenant), _currentUser.TenantId);

            languageId = tenant.DefaultLanguageId;
        }

        return await _context.Languages
                .Where(l => l.Id == languageId)
                .Select(l => new LanguageOptionDto { Id = l.Id, Code = l.Code, Name = l.Name,IsRightToLeft=l.IsRightToLeft })
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Language), languageId);
    }
}
