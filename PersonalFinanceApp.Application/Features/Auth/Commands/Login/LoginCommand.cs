using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Features.Auth.Common;

namespace PersonalFinanceApp.Application.Features.Auth.Commands.Login;

public class LoginCommand : IRequest<AuthResultDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

}
