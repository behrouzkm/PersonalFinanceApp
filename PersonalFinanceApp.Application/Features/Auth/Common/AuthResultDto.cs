using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Features.Auth.Common;

public record AuthResultDto(string Token, DateTime ExpiresAtUtc);
