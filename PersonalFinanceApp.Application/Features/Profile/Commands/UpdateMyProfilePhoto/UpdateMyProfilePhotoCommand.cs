using System.IO;
using MediatR;

namespace PersonalFinanceApp.Application.Features.Profile.Commands.UpdateMyProfilePhoto;

public class UpdateMyProfilePhotoCommand : IRequest
{
    public Stream Content { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}
