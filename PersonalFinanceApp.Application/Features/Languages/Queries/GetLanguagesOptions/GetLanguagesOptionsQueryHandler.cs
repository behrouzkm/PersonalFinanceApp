using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.Languages.Common;


namespace PersonalFinanceApp.Application.Features.Languages.Queries.GetLanguagesOptions;

public class GetLanguagesOptionsQueryHandler : IRequestHandler<GetLanguagesOptionsQuery, List<LanguageOptionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLanguagesOptionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LanguageOptionDto>> Handle(GetLanguagesOptionsQuery request,
                        CancellationToken cancellationToken)
    {
        var options = await _context.Languages
            .AsNoTracking()
            .Where(r => r.IsActive)
            .OrderBy(o => o.DisplayOrder)
            .Select(r => new LanguageOptionDto
            {
                Id = r.Id,
                Name = r.Name,
                Code = r.Code,
            })
            .ToListAsync(cancellationToken);

        return options;
    }
}
