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
    private readonly IReorderService _reorderService;

    public DeleteLanguageCommandHandler(IApplicationDbContext context,IReorderService reorderService )
    {
        _context = context;
        _reorderService=reorderService;
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

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Phase 1: the row must actually be gone before anything shifts into its slot.
            _context.Languages.Remove(language);
            await _context.SaveChangesAsync(cancellationToken);

            // Phase 2: now the slot is genuinely vacant.
            await _reorderService.CloseGapAsync(language, cancellationToken, null);
            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
