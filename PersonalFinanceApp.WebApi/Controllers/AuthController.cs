using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Application.Features.Auth.Commands.CreateTenantUser;
using PersonalFinanceApp.Application.Features.Auth.Commands.Login;
using PersonalFinanceApp.Application.Features.Auth.Commands.Register;

namespace PersonalFinanceApp.WebApi.Controllers;


public class AuthController : BaseApiController
{

    public AuthController(IMediator mediator): base(mediator)
    {
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<Guid>> Register(RegisterCommand command, CancellationToken cancellationToken)
    {
        var userId = await _mediator.Send(command,cancellationToken);

        return Ok(userId);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<string>> Login (LoginCommand command,CancellationToken cancellationToken)
    {
        var token = await _mediator.Send(command,cancellationToken);

        return Ok(token);
    }

    [HttpPost("users")]
    [Authorize]
    public async Task<ActionResult<Guid>> CreateTenantUser(CreateTenantUserCommand command,
                        CancellationToken cancellationToken)
    {
        var userId = await _mediator.Send(command,cancellationToken);

        return Ok(userId);
    }

}
