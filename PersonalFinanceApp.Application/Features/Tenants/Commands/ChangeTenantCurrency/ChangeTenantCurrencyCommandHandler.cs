using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.Tenants.Commands.ChangeTenantCurrency;

public class ChangeTenantCurrencyCommandHandler : IRequestHandler<ChangeTenantCurrencyCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeTenantCurrencyCommandHandler(
        IApplicationDbContext context, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _context = context;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ChangeTenantCurrencyCommand request, CancellationToken cancellationToken)
    {
        var currency = await _context.Currencies
                .FirstOrDefaultAsync(c => c.Id == request.CurrencyId, cancellationToken)
            ?? throw new NotFoundException(nameof(Currency), request.CurrencyId);

        if (!currency.IsActive)
            throw new BusinessRuleException(ApplicationErrorCodes.Currency.CurrencyDeactivated, request.CurrencyId);

        var tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Id == _currentUser.TenantId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tenant), _currentUser.TenantId);

        tenant.ChangeDefaultCurrency(request.CurrencyId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
