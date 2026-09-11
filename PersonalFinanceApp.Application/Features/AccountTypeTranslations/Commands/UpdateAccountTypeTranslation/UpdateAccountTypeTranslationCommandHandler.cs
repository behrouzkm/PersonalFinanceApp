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

namespace PersonalFinanceApp.Application.Features.AccountTypeTranslations.Commands.UpdateAccountTypeTranslation;

public class UpdateAccountTypeTranslationCommandHandler : IRequestHandler<UpdateAccountTypeTranslationCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAccountTypeTranslationCommandHandler(IApplicationDbContext context,IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork=unitOfWork;
    }

    public async Task Handle(UpdateAccountTypeTranslationCommand request, CancellationToken cancellationToken)
    {
        var accountType = await _context.AccountTypes
          .FirstOrDefaultAsync(x => x.Id == request.AccountTypeId, cancellationToken)
          ?? throw new NotFoundException(nameof(AccountType), request.AccountTypeId);

        var language = await _context.Languages
            .FirstOrDefaultAsync(r => r.Id == request.LanguageId, cancellationToken)
            ?? throw new NotFoundException(nameof(Language), request.LanguageId);

        if (!language.IsActive)
            throw new BusinessRuleException(ApplicationErrorCodes.AccountTypeTranslation.LanguageIsNoActive);

        var att = await _context.AccountTypeTranslations
            .FirstOrDefaultAsync(r=>r.Id==request.Id,cancellationToken) ??
            throw new NotFoundException(nameof(AccountTypeTranslation),request.Id);


        var duplicateExist = await _context.AccountTypeTranslations
                 .AnyAsync(c => c.Id != request.Id &&
                    c.AccountTypeId == request.AccountTypeId &&
                    c.LanguageId == request.LanguageId, cancellationToken);

        if (duplicateExist)
            throw new BusinessRuleException(ApplicationErrorCodes.AccountTypeTranslation.DuplicateRecord, request.AccountTypeId, request.LanguageId);

        att.UpdateAccountTypeTranslation(request.AccountTypeId,request.LanguageId,request.Translation,request.Translation);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

    }
}
