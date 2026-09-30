using MediatR;

namespace PersonalFinanceApp.Application.Features.Tenants.Commands.ChangeTenantLanguage;

public class ChangeTenantLanguageCommand : IRequest
{
    public int LanguageId { get; set; }
}

