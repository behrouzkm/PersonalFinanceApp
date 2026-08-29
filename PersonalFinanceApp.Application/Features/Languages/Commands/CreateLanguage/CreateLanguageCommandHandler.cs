using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.Languages.Commands.CreateLanguage;

public class CreateLanguageCommandHandler : IRequestHandler<CreateLanguageCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateLanguageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
    {
        var duplicateExist = await _context.Languages
                .AnyAsync(c => c.Code == request.Code || c.Name == request.Name, cancellationToken);

        if (duplicateExist)
            throw new BusinessRuleException(ApplicationErrorCodes.Language.DuplicateCodeOrName, request.Code, request.Name);

        var maxDisplayOrder = await _context.Languages.MaxAsync(c => (int?)c.DisplayOrder, cancellationToken) ?? 0;

        var language = new Language(
            request.Code,
            request.Name,
            request.IsActive,
            maxDisplayOrder+1,
            request.IsRightToLeft);

        await _context.Languages.AddAsync(language, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return language.Id;

    }
}
