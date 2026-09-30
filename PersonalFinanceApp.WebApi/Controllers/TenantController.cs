using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Application.Common.Constants;
using PersonalFinanceApp.Application.Features.Tenants.Commands.ChangeTenantCurrency;
using PersonalFinanceApp.Application.Features.Tenants.Commands.ChangeTenantLanguage;
using PersonalFinanceApp.Application.Features.Tenants.Queries.GetMyTenantLanguage;

namespace PersonalFinanceApp.WebApi.Controllers;

// No [Authorize] needed here explicitly — BaseApiController already applies
// it at class level, same as every other non-admin controller in this project.
public class TenantController : BaseApiController
{
    public TenantController(IMediator mediator) : base(mediator) { }

    [HttpGet("language")]
    public async Task<IActionResult> GetDefaultLanguage(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMyTenantLanguageQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPut("language")]
    [Authorize(Roles = Roles.TenantAdministrators)]
    public async Task<IActionResult> ChangeDefaultLanguage(
        [FromBody] ChangeTenantLanguageCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPut("currency")]
    [Authorize(Roles = Roles.TenantAdministrators)]
    public async Task<IActionResult> ChangeCurrency(
        [FromBody] ChangeTenantCurrencyCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
