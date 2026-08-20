using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Application.Features.Auth.Commands.Login;
using PersonalFinanceApp.Application.Features.Auth.Commands.Register;

namespace PersonalFinanceApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<Guid>> Register(RegisterCommand command, CancellationToken cancellationToken)
    {
        var userId = await _mediator.Send(command,cancellationToken);

        return Ok(userId);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login (LoginCommand command,CancellationToken cancellationToken)
    {
        var token = await _mediator.Send(command,cancellationToken);

        return Ok(token);
    }

}
