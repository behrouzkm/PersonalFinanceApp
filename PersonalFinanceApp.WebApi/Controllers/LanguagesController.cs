using MediatR;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Application.Features.Languages.Common;
using PersonalFinanceApp.Application.Features.Languages.Queries.GetLanguagesOptions;

namespace PersonalFinanceApp.WebApi.Controllers;

public class LanguagesController : BaseApiController
{
    public LanguagesController(IMediator mediator) : base(mediator)
    {
    }



    [HttpGet]
    public async Task<ActionResult<List<LanguageOptionDto>>> GetOptionList([FromQuery] GetLanguagesOptionsQuery query,
                        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

}
