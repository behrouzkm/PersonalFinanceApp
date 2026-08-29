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

namespace PersonalFinanceApp.Application.Features.Languages.Commands.DeleteLanguage;

public class DeleteLanguageCommandHandler : IRequestHandler<DeleteLanguageCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteLanguageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteLanguageCommand request, CancellationToken cancellationToken)
    {
        var language = await _context.Languages
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Language), request.Id);


        var languageInUse = await _context.Tenants
                .AnyAsync(r => r.DefaultLanguageId == request.Id, cancellationToken);

        languageInUse |= await _context.AccountTypeTranslations
                .AnyAsync(r => r.LanguageId == request.Id, cancellationToken);

        languageInUse |= await _context.DocumentTypeTranslations
                .AnyAsync(r => r.LanguageId == request.Id, cancellationToken);

        if (languageInUse)
            throw new BusinessRuleException(ApplicationErrorCodes.Language.LanguageInUse, request.Id, language.Name);

        var currentDisplayOrder = language.DisplayOrder;

        _context.Languages.Remove(language);

        var nextRecords = await _context.Languages
                .Where(r => r.DisplayOrder > currentDisplayOrder)
                .ToListAsync(cancellationToken);

        foreach (var lang in nextRecords)
            lang.DecrementDisplayOrder();


        await _context.SaveChangesAsync(cancellationToken);

    }
}
