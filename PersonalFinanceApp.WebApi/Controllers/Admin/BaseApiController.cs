using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PersonalFinanceApp.WebApi.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize]
public class AdminBaseApiController : ControllerBase
{
    protected readonly IMediator _mediator;


    protected AdminBaseApiController(IMediator mediator)
    {
        _mediator = mediator;
    }

}
