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

public class CreateLanguageCommandHandler : IRequestHandler<CreateLanguageCommand, byte>
{
    private readonly IApplicationDbContext _context;

    public CreateLanguageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<byte> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
    {
        var duplicateExist = await _context.Languages
                .AnyAsync(c => c.Code == request.Code || c.Name == request.Name, cancellationToken);

        if (duplicateExist)
            throw new BusinessRuleException(ApplicationErrorCodes.Language.DuplicateCodeOrName, request.Code, request.Name);

        var newId = await _context.Languages.MaxAsync(c => c.Id, cancellationToken) + 1;

        var Language = new Language(
            (byte)newId,
            request.Code,
            request.Name,
            request.IsActive,
            (byte)newId,
            request.IsRightToLeft);

        await _context.Languages.AddAsync(Language, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return (byte)newId;

    }
}
