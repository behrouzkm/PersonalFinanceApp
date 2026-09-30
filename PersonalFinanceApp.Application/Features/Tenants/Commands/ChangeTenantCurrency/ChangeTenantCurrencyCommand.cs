using MediatR;

namespace PersonalFinanceApp.Application.Features.Tenants.Commands.ChangeTenantCurrency;

public class ChangeTenantCurrencyCommand : IRequest
{
    public int CurrencyId { get; set; }
}
