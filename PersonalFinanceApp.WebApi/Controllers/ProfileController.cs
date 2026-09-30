using MediatR;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Application.Features.Profile.Commands.ChangeMyPassword;
using PersonalFinanceApp.Application.Features.Profile.Commands.UpdateMyProfile;
using PersonalFinanceApp.Application.Features.Profile.Commands.UpdateMyProfilePhoto;
using PersonalFinanceApp.Application.Features.Profile.Queries.GetMyProfile;

namespace PersonalFinanceApp.WebApi.Controllers;

public class ProfileController : BaseApiController
{
    public ProfileController(IMediator mediator) : base(mediator) { }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMyProfileQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(
        [FromBody] UpdateMyProfileCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    // Matches how UploadAttachment's real controller action almost certainly
    // shapes multipart uploads already - adjust the [FromForm] binding here to
    // match that existing action's exact signature if it differs.
    [HttpPost("photo")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UpdatePhoto(IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();

        await _mediator.Send(new UpdateMyProfilePhotoCommand
        {
            Content = stream,
            FileName = file.FileName,
            ContentType = file.ContentType
        }, cancellationToken);

        return NoContent();
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangeMyPasswordCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
