using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Features.Languages.Common;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.Languages.Queries.GetLanguageByCode;

public class GetLanguageByCodeQueryHandler : IRequestHandler<GetLanguageByCodeQuery, LanguageDto>
{
    private readonly IApplicationDbContext _context;

    public GetLanguageByCodeQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LanguageDto> Handle(GetLanguageByCodeQuery request, CancellationToken cancellationToken)
    {
        var language = await _context.Languages
                .FirstOrDefaultAsync(d => d.Code == request.Code, cancellationToken)
            ?? throw new NotFoundException(nameof(Language), request.Code);

        if (!language.IsActive)
            throw new BusinessRuleException(ApplicationErrorCodes.Language.LanguageDeactivated);

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
