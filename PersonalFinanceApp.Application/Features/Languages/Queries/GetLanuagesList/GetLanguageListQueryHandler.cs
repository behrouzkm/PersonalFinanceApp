using MediatR;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.Languages.Common;


namespace PersonalFinanceApp.Application.Features.Languages.Queries.GetLanguagesList;

public class GetLanguageListQueryHandler : IRequestHandler<GetLanguagesListQuery, PaginatedList<LanguageDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLanguageListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<LanguageDto>> Handle(GetLanguagesListQuery request,
                        CancellationToken cancellationToken)
    {
        var query = _context.Languages;

        var projection = query
            .OrderBy(o => o.DisplayOrder)
            .Select(r => new LanguageDto
            {
                Id = r.Id,
                Name = r.Name,
                Code = r.Code,
                IsActive = r.IsActive,
                IsRightToLeft=r.IsRightToLeft
            });

        return await PaginatedList<LanguageDto>.CreateAsync(projection, request.PageNumber,
                       request.PageSize, cancellationToken);
    }
}
