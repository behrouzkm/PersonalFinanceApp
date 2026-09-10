using MediatR;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.AccountTypeTranslations.Common;


namespace PersonalFinanceApp.Application.Features.AccountTypeTranslations.Queries.GetAccountTypeTranslationsList;

public class GetAccountTypeTranslationsListQueryHandler : IRequestHandler<GetAccountTypeTranslationsListQuery, PaginatedList<AccountTypeTranslationDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAccountTypeTranslationsListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<AccountTypeTranslationDto>> Handle(GetAccountTypeTranslationsListQuery request,
                        CancellationToken cancellationToken)
    {
        var query = _context.AccountTypeTranslations.AsQueryable();

        if (request.AccountTypeId.HasValue)
            query = query.Where(r => r.AccountTypeId == request.AccountTypeId.Value);

        if (request.LanguageId.HasValue)
            query = query.Where(r => r.LanguageId == request.LanguageId);

        var projection = query
            .Select(r => new AccountTypeTranslationDto
            {
                Id = r.Id,
                AccountTypeId = r.AccountTypeId,
                LanguageId = r.LanguageId,
                Translation = r.Translation,
                Description = r.Description
            });

        return await PaginatedList<AccountTypeTranslationDto>.CreateAsync(projection, request.PageNumber,
                       request.PageSize, cancellationToken);
    }
}
