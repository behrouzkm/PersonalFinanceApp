using MediatR;
using PersonalFinanceApp.Application.Features.Profile.Common;

namespace PersonalFinanceApp.Application.Features.Profile.Queries.GetMyProfile;

public class GetMyProfileQuery : IRequest<MyProfileDto> { }
