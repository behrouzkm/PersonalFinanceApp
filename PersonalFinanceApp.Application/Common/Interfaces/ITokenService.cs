using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Common.Interfaces;

public interface ITokenService
{
    TokenResult GenerateToken(Guid userId, Guid tenantId, string email, IEnumerable<string> roles);
}

public record TokenResult(string Token, DateTime ExpiresAtUtc);
