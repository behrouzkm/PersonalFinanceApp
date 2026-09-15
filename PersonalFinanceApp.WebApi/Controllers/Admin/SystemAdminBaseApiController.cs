using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Application.Common.Constants;

namespace PersonalFinanceApp.WebApi.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = Roles.SystemAdministrators)]
public class SystemAdminBaseApiController : ControllerBase
{
    protected readonly IMediator _mediator;


    protected SystemAdminBaseApiController(IMediator mediator)
    {
        _mediator = mediator;
    }

}
