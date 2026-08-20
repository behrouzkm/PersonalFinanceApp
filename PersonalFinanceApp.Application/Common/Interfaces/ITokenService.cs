using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateToken(Guid userId, Guid tenantId, string email);
}
