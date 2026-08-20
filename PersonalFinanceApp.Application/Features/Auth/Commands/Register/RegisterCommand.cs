using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.Auth.Commands.Register;

// Registration creates a Tenant and its first ApplicationUser together, in one
// operation - same "create the dependency alongside the entity that needs it"
// pattern used for Person+LedgerAccount elsewhere in this project. TenantName,
// DefaultLanguageId, DefaultCurrencyId are exactly what Tenant's own constructor
public class RegisterCommand : IRequest<Guid>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string TenantName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public byte DefaultLanguageId { get; set; }
    public byte DefaultCurrencyId { get; set; }

}
