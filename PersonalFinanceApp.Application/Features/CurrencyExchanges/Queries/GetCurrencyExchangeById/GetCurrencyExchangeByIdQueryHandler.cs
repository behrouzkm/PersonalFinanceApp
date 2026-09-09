using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Features.CurrencyExchanges.Common;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.CurrencyExchanges.Queries.GetCurrencyExchangeById;

public class GetCurrencyExchangeByIdQueryHandler : IRequest<CurrencyExchangeDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAccountingLookupService _lookupService;
    private readonly IAttachmentService _attachmentService;

    public GetCurrencyExchangeByIdQueryHandler(
            IApplicationDbContext context,
            IAccountingLookupService lookupService,
            IAttachmentService attachmentService)
    {
        _context = context;
        _lookupService = lookupService;
        _attachmentService = attachmentService;
    }

    public async Task<CurrencyExchangeDto> Handle(GetCurrencyExchangeByIdQuery request, CancellationToken cancellationToken)
    {
        var query = await _context.CurrencyExchanges
                .Include(r => r.FromDocument).ThenInclude(d => d.Currency)
                .Include(r => r.ToDocument).ThenInclude(d => d.Currency)
                .Include(r => r.FromDocument).ThenInclude(d => d.Entries)
                .Include(r => r.ToDocument).ThenInclude(d => d.Entries)
                .FirstOrDefaultAsync(r => r.Id == request.CurrencyExchangeId, cancellationToken)
            ?? throw new NotFoundException(nameof(CurrencyExchange), request.CurrencyExchangeId);

        var fromLedgerAccountId = query.FromDocument.Entries.First(e => e.Credit > 0).LedgerAccountId;
        var toLedgerAccountId = query.ToDocument.Entries.First(e => e.Debit > 0).LedgerAccountId;

        var (fromFundSource, _) = await _lookupService.GetFundSourceByLedgerAccountIdAsync(fromLedgerAccountId,
                                    cancellationToken);

        var (toFundSource, _) = await _lookupService.GetFundSourceByLedgerAccountIdAsync(toLedgerAccountId,
                                    cancellationToken);

        var attachments = await _attachmentService.GetForOwnerAsync(
                   AttachmentOwnerType.CurrencyExchange, request.CurrencyExchangeId, cancellationToken);

        var dto = new CurrencyExchangeDto
        {
            Id = query.Id,
            RowVersion = query.RowVersion,
            FromLedgerAccountId = fromLedgerAccountId,
            FromFundSourceName = fromFundSource.DisplayName,
            FromCurrencyId = query.FromDocument.CurrencyId,
            FromCurrencyCode = query.FromDocument.Currency.Code,
            FromAmount = query.FromDocument.Entries.First(e => e.Credit > 0).Credit,
            ToLedgerAccountId = toLedgerAccountId,
            ToFundSourceName = toFundSource.DisplayName,
            ToCurrencyId = query.ToDocument.CurrencyId,
            ToCurrencyCode = query.ToDocument.Currency.Code,
            ToAmount = query.ToDocument.Entries.First(e => e.Debit > 0).Debit,
            ExchangeRate = query.ExchangeRate,
            ExchangeDate = query.FromDocument.DocumentDate,
            Description = query.Description,
            Attachments = attachments
        };


        return dto;


    }
}
