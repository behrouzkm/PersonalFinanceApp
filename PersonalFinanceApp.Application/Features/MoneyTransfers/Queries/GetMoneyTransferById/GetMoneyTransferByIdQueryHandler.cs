using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Features.Common;
using PersonalFinanceApp.Application.Features.MoneyTransfers.Common;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Queries.GetMoneyTransferById;

public class GetMoneyTransferByIdQueryHandler : IRequestHandler<GetMoneyTransferByIdQuery, MoneyTransferDetailsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAttachmentService _attachmentService;
    private readonly IAccountingLookupService _accountingLookupService;
    private readonly IUserLookupService _userLookupService;
    private readonly ICurrentUserService _currentUser;

    public GetMoneyTransferByIdQueryHandler(
        IApplicationDbContext context,
        IAttachmentService attachmentService,
        IAccountingLookupService accountingLookupService,
        IUserLookupService userLookupService,
        ICurrentUserService currentUser)
    {
        _context = context;
        _attachmentService = attachmentService;
        _accountingLookupService = accountingLookupService;
        _userLookupService = userLookupService;
        _currentUser = currentUser;
    }



    public async Task<MoneyTransferDetailsDto> Handle(
    GetMoneyTransferByIdQuery request,
    CancellationToken cancellationToken)
    {
        var document = await _context.AccountingDocuments
            .Include(d => d.Entries)
                .ThenInclude(e => e.LedgerAccount)
            .Include(d => d.Currency)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                d => d.Id == request.MoneyTransferDocumentId &&
                     d.DocumentType == DocumentType.MoneyTransfer,
                cancellationToken)
            ?? throw new NotFoundException(
                nameof(AccountingDocument),
                request.MoneyTransferDocumentId);

        var entries = document.Entries.ToList();

        var creditEntry = entries.FirstOrDefault(e => e.Credit > 0)
            ?? throw new BusinessRuleException(ApplicationErrorCodes.MoneyTransfer.MissingCreditEntry);

        var debitEntry = entries.FirstOrDefault(e => e.Debit > 0)
            ?? throw new BusinessRuleException(ApplicationErrorCodes.MoneyTransfer.MissingDebitEntry);

        var fromLedgerAccountId = creditEntry.LedgerAccountId;
        var toLedgerAccountId = debitEntry.LedgerAccountId;

        var (fromFundSource, fromLedgerAccount) = await _accountingLookupService
            .GetFundSourceByLedgerAccountIdAsync(fromLedgerAccountId, cancellationToken);

        var (toFundSource, toLedgerAccount) = await _accountingLookupService
            .GetFundSourceByLedgerAccountIdAsync(toLedgerAccountId, cancellationToken);

        var (fromPersonId, fromBankAccountId, fromCashAccountId) =
            MapFundSourceId(fromFundSource, fromLedgerAccount);

        var (toPersonId, toBankAccountId, toCashAccountId) =
            MapFundSourceId(toFundSource, toLedgerAccount);

        var tenantUsers = await _userLookupService.GetTenantUsersAsync(_currentUser.TenantId, cancellationToken);
        var usersById = tenantUsers.ToDictionary(u => u.Id);

        usersById.TryGetValue(document.CreatedBy, out var createdByUser);
        var lastModifiedByUser = document.LastModifiedBy.HasValue
            ? usersById.GetValueOrDefault(document.LastModifiedBy.Value)
            : null;

        var attachments = await _attachmentService.GetForOwnerAsync(
            AttachmentOwnerType.AccountingDocument,
            document.Id,
            cancellationToken);

        return new MoneyTransferDetailsDto
        {
            MoneyTransferDocumentId = document.Id,
            RowVersion = document.RowVersion,

            TransferDate = document.DocumentDate,
            CurrencyId = document.CurrencyId,
            CurrencyCode = document.Currency.Code,
            CurrencySymbol = document.Currency.Symbol,
            CurrencyDecimalPlaces = document.Currency.DecimalPlaces,

            Amount = debitEntry.Debit,
            Description = document.Description,

            FromLedgerAccountId = fromLedgerAccountId,
            ToLedgerAccountId = toLedgerAccountId,

            FromDisplayName = creditEntry.LedgerAccount.Name,
            ToDisplayName = debitEntry.LedgerAccount.Name,

            FromPersonId = fromPersonId,
            FromBankAccountId = fromBankAccountId,
            FromCashAccountId = fromCashAccountId,

            ToPersonId = toPersonId,
            ToBankAccountId = toBankAccountId,
            ToCashAccountId = toCashAccountId,

            CreatedAt = document.CreatedAt,
            CreatedBy = document.CreatedBy,
            CreatedByUserName = createdByUser is null
                ? null
                : $"{createdByUser.FirstName} {createdByUser.LastName}".Trim(),

            LastModifiedAt = document.LastModifiedAt,
            LastModifiedBy = document.LastModifiedBy,
            LastModifiedByUserName = lastModifiedByUser is null
                ? null
                : $"{lastModifiedByUser.FirstName} {lastModifiedByUser.LastName}".Trim(),

            Attachments = attachments
        };
    }

    private static (Guid? PersonId, Guid? BankAccountId, Guid? CashAccountId) MapFundSourceId(
        IFundSource fundSource, LedgerAccount ledgerAccount)
    {
        return ledgerAccount.AccountType.Category switch
        {
            AccountCategory.PersonAccount => (((Person)fundSource).Id, null, null),
            AccountCategory.BankAccount => (null, ((MonetaryAccount)fundSource).Id, null),
            AccountCategory.CashAccount => (null, null, ((MonetaryAccount)fundSource).Id),
            _ => (null, null, null)
        };
    }



}
