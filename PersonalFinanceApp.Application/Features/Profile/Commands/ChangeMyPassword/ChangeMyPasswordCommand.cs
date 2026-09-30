using MediatR;

namespace PersonalFinanceApp.Application.Features.Profile.Commands.ChangeMyPassword;

public class ChangeMyPasswordCommand : IRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
