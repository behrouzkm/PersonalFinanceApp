using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Features.Languages.Common;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.Languages.Queries.GetLanguageById;

public class GetLanguageByIdQueryHandler : IRequestHandler<GetLanguageByIdQuery, LanguageDto>
{
    private readonly IApplicationDbContext _context;

    public GetLanguageByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LanguageDto> Handle(GetLanguageByIdQuery request, CancellationToken cancellationToken)
    {
        var language = await _context.Languages
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Language), request.Id);

        return new LanguageDto
        {
            Id = language.Id,
            Name = language.Name,
            Code = language.Code,
            IsActive = language.IsActive,
            IsRightToLeft = language.IsRightToLeft
        };

    }
}
