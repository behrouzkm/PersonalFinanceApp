using MediatR;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Application.Features.Common;
using PersonalFinanceApp.Application.Features.Currencies.Common;
using PersonalFinanceApp.Application.Features.Currencies.Queries.GetCurrenciesOptions;
using PersonalFinanceApp.Application.Features.Languages.Common;
using PersonalFinanceApp.Application.Features.Languages.Queries.GetLanguagesOptions;
using PersonalFinanceApp.Application.Features.MonetaryAccounts.Queries.GetMonetaryAccountOptions;

namespace PersonalFinanceApp.WebApi.Controllers;

public class LookupController : BaseApiController
{
    public LookupController(IMediator mediator) : base(mediator)
    {
    }



    [HttpGet("language-options")]
    public async Task<ActionResult<List<LanguageOptionDto>>> GetLanguageOptions([FromQuery] GetLanguagesOptionsQuery query,
                        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("currency-options")]
    public async Task<ActionResult<List<CurrencyOptionDto>>> GetCurrencyOptions([FromQuery] GetCurrenciesOptionsQuery query,
                        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("monetary-account-options")]
    public async Task<ActionResult<List<MonetaryAccountOptionDto>>> GetMonetaryAccountOptions([FromQuery] GetMonetaryAccountOptionsQuery query,
                    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

}
