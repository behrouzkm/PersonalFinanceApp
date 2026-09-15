using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Common.Constants;

public static class Roles
{
    public const string SystemAdministrators = "SystemAdministrators"; // global: manages Language/Currency/AccountType, cross-tenant
    public const string TenantAdministrators = "TenantAdministrators"; // owns a tenant, can invite Users into it
    public const string Users = "Users";                               // regular tenant member, no admin surface
}
