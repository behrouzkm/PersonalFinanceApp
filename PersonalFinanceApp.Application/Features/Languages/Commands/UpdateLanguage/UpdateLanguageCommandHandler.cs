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

namespace PersonalFinanceApp.Application.Features.Languages.Commands.UpdateLanguage;

public class UpdateLanguageCommandHandler : IRequestHandler<UpdateLanguageCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateLanguageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateLanguageCommand request, CancellationToken cancellationToken)
    {
        var language = await _context.Languages
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Language), request.Id);

        var duplicateExist = await _context.Languages
                .AnyAsync(
                    c => c.Id != request.Id
                    && (c.Code == request.Code || c.Name == request.Name), cancellationToken);

        if (duplicateExist)
            throw new BusinessRuleException(ApplicationErrorCodes.Language.DuplicateCodeOrName, request.Code, request.Name);


        language.UpdateLanguage(request.Code, request.Name, request.IsActive, request.IsRightToLeft);

        await _context.SaveChangesAsync(cancellationToken);

    }
}
