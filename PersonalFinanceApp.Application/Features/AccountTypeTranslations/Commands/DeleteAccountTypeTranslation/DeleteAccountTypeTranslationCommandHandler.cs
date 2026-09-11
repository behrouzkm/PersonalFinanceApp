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

namespace PersonalFinanceApp.Application.Features.AccountTypeTranslations.Commands.DeleteAccountTypeTranslation;

public class DeleteAccountTypeTranslationCommandHandler : IRequestHandler<DeleteAccountTypeTranslationCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAccountTypeTranslationCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteAccountTypeTranslationCommand request, CancellationToken cancellationToken)
    {
        var att = await _context.AccountTypeTranslations
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(AccountTypeTranslation), request.Id);

        _context.AccountTypeTranslations.Remove(att);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
