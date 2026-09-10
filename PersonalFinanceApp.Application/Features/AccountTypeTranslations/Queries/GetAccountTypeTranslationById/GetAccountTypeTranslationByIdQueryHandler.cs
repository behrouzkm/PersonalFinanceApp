using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Features.AccountTypeTranslations.Common;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.AccountTypeTranslations.Queries.GetAccountTypeTranslationById;

public class GetAccountTypeTranslationByIdQueryHandler : IRequestHandler<GetAccountTypeTranslationByIdQuery, AccountTypeTranslationDto>
{
    private readonly IApplicationDbContext _context;

    public GetAccountTypeTranslationByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AccountTypeTranslationDto> Handle(GetAccountTypeTranslationByIdQuery request, CancellationToken cancellationToken)
    {
        var att = await _context.AccountTypeTranslations
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(AccountTypeTranslation), request.Id);

        return new AccountTypeTranslationDto
        {
            Id = att.Id,
            AccountTypeId=att.AccountTypeId,
            LanguageId=att.LanguageId,
            Translation=att.Translation,
            Description=att.Description
        };

    }
}
