using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Features.Common;
using PersonalFinanceApp.Application.Features.MoneyTransfers.Common;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Queries.GetMoneyTransferById;

public class GetMoneyTransferByIdQueryHandler : IRequestHandler<GetMoneyTransferByIdQuery, MoneyTransferDetailsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAttachmentService _attachmentService;

    public GetMoneyTransferByIdQueryHandler(IApplicationDbContext context, IAttachmentService attachmentService)
    {
        _context = context;
        _attachmentService = attachmentService;
    }

    public async Task<MoneyTransferDetailsDto> Handle(GetMoneyTransferByIdQuery request, CancellationToken cancellationToken)
    {
        var document = await _context.AccountingDocuments
                .Include(i => i.Entries)
                .FirstOrDefaultAsync(d => d.Id == request.MoneyTransferDocumentId
                    && d.DocumentType == Domain.Enums.DocumentType.MoneyTransfer, cancellationToken)
            ?? throw new NotFoundException(nameof(AccountingDocument), request.MoneyTransferDocumentId);


        var activeEntries = document.Entries.ToList();
        var ledgerAccountIds = activeEntries.Select(s => s.LedgerAccountId).Distinct().ToList();

        // Resolve which ledger accounts belong to a MonetaryAccount vs a Person vs a
        // plain expense category, so each entry can be routed into the right list.
        var monetaryAccountByLedgerId = await _context.MonetaryAccounts
                .Where(m => ledgerAccountIds.Contains(m.LedgerAccountId))
                .ToDictionaryAsync(m => m.LedgerAccountId, cancellationToken);

        var personsByLedgerId = await _context.Persons
                 .Where(p => ledgerAccountIds.Contains(p.LedgerAccountId))
                 .ToDictionaryAsync(p => p.LedgerAccountId, cancellationToken);

        var attachments = await _attachmentService.GetForOwnerAsync(
            AttachmentOwnerType.AccountingDocument, document.Id, cancellationToken);

        var dto = new MoneyTransferDetailsDto
        {
            AccountingDocumentId = document.Id,
            RowVersion = document.RowVersion,
            TransferDate = document.DocumentDate,
            CurrencyId = document.CurrencyId,
            Description = document.Description,
            Amount = activeEntries.First(r => r.Debit > 0).Debit,
            FromMonetaryAccountId = monetaryAccountByLedgerId.FirstOrDefault(r => r.Key == activeEntries.First(r => r.Credit > 0).LedgerAccountId).Value?.Id,
            ToMonetaryAccountId = monetaryAccountByLedgerId.FirstOrDefault(r => r.Key == activeEntries.First(r => r.Debit > 0).LedgerAccountId).Value?.Id,
            FromPersonId = personsByLedgerId.FirstOrDefault(r => r.Key == activeEntries.First(r => r.Credit > 0).LedgerAccountId).Value?.Id,
            ToPersonId = personsByLedgerId.FirstOrDefault(r => r.Key == activeEntries.First(r => r.Debit > 0).LedgerAccountId).Value?.Id,
            Attachments = attachments
        };



        return dto;


    }
}
