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

    public GetMyTenantLanguageQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<LanguageOptionDto> Handle(GetMyTenantLanguageQuery request, CancellationToken cancellationToken)
    {
        // Global query filter already scopes Tenants... except Tenant itself
        // isn't a BaseAuditableEntity (it's the tenant boundary, not owned by
        // one), so this is the one place TenantId is looked up directly rather
        // than relied on to be pre-filtered.
        var tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Id == _currentUser.TenantId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tenant), _currentUser.TenantId);

        var language = await _context.Languages
                .Where(l => l.Id == tenant.DefaultLanguageId)
                .Select(l => new LanguageOptionDto
                {
                    Id = l.Id,
                    Code = l.Code,
                    Name = l.Name,
                })
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Language), tenant.DefaultLanguageId);

        return language;
    }
}
