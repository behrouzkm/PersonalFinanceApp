
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.Tenants.Commands.ChangeTenantLanguage;

public class ChangeTenantLanguageCommandHandler : IRequestHandler<ChangeTenantLanguageCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeTenantLanguageCommandHandler(
        IApplicationDbContext context, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _context = context;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ChangeTenantLanguageCommand request, CancellationToken cancellationToken)
    {
        var language = await _context.Languages
                .FirstOrDefaultAsync(l => l.Id == request.LanguageId, cancellationToken)
            ?? throw new NotFoundException(nameof(Language), request.LanguageId);

        if (!language.IsActive)
            throw new BusinessRuleException(ApplicationErrorCodes.Language.LanguageDeactivated, request.LanguageId);

        var tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Id == _currentUser.TenantId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tenant), _currentUser.TenantId);

        tenant.ChangeDefaultLanguage(request.LanguageId);


        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
