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

namespace PersonalFinanceApp.Application.Features.Languages.Commands.ReorderLanguage;

public class ReorderLanguageCommandHandler : IRequestHandler<ReorderLanguageCommand>
{
    private readonly IApplicationDbContext _context;

    public ReorderLanguageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ReorderLanguageCommand request, CancellationToken cancellationToken)
    {
        var language = await _context.Languages
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Language), request.Id);


        var languagesCount = await _context.Languages
                .OrderBy(o => o.DisplayOrder)
                .CountAsync(cancellationToken);

        if (languagesCount <= 1)
            return;

        var oldPosition = language.DisplayOrder;

        var newPosition = Math.Clamp(request.NewDisplayOrder, 1, languagesCount);

        if (oldPosition == newPosition)
            return;

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Move the selected language temporarily outside the normal range.
            language.SetDisplayOrder(languagesCount + 1);
            await _context.SaveChangesAsync(cancellationToken);


            // 2. Shift the other languages
            if (newPosition < oldPosition)
            {
                var inRangeLanguages = await _context.Languages
                    .Where(r => r.DisplayOrder >= oldPosition && r.DisplayOrder <= newPosition)
                    .ToListAsync(cancellationToken);

                foreach (var item in inRangeLanguages)
                {
                    item.MoveUp();
                }
            }
            else
            {
                var inRangeLanguages = await _context.Languages
                    .Where(r => r.DisplayOrder >= newPosition && r.DisplayOrder <= oldPosition)
                    .ToListAsync(cancellationToken);

                foreach (var item in inRangeLanguages)
                {
                    item.MoveDown();
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            // 3. Put the selected language in its new position.
            language.SetDisplayOrder(newPosition);
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
