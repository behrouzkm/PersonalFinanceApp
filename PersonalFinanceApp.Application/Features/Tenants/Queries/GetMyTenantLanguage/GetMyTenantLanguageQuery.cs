using MediatR;
using PersonalFinanceApp.Application.Features.Languages.Common;

namespace PersonalFinanceApp.Application.Features.Tenants.Queries.GetMyTenantLanguage;

// No parameters: the tenant is resolved from ICurrentUserService, same as
// every other handler in the codebase resolves the current tenant — never
// pass TenantId from the client.
public class GetMyTenantLanguageQuery : IRequest<LanguageOptionDto> { }

