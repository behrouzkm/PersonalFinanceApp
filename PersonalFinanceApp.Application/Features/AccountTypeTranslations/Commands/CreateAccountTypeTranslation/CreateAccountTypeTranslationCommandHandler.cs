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

namespace PersonalFinanceApp.Application.Features.AccountTypeTranslations.Commands.CreateAccountTypeTranslation;

public class CreateAccountTypeTranslationCommandHandler : IRequestHandler<CreateAccountTypeTranslationCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateAccountTypeTranslationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateAccountTypeTranslationCommand request, CancellationToken cancellationToken)
    {
        var duplicateExist = await _context.AccountTypeTranslations
                .AnyAsync(c => c.AccountTypeId == request.AccountTypeId && c.LanguageId == request.LanguageId, cancellationToken);

        if (duplicateExist)
            throw new BusinessRuleException(ApplicationErrorCodes.AccountTypeTranslation.DuplicateRecord, request.AccountTypeId, request.LanguageId);


        var att = new AccountTypeTranslation(
                request.AccountTypeId,
                request.LanguageId,
                request.Translation,
                request.Description
            );

        await _context.AccountTypeTranslations.AddAsync(att, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return att.Id;

    }
}
