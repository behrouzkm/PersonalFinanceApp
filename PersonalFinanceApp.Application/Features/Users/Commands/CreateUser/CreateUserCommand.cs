using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommand : IRequest<Guid>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    // No TenantId here - deliberately. It comes from ICurrentUserService,
    // the same "never accept client-supplied X for something derived from
    // context" rule as LedgerAccountId elsewhere in this project. A tenant
    // admin can only create users in their own tenant.
}
