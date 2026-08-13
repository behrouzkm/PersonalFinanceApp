using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Features.Common;
using PersonalFinanceApp.Application.Features.Incomes.Common;

namespace PersonalFinanceApp.Application.Features.Incomes.Commands.UpdateIncome;

public class UpdateIncomeCommand : IRequest, IIncomeRequest
{
    public Guid AccountingDocumentId { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateOnly DocumentDate { get; set; }

    public byte CurrencyId { get; set; }

    public string? Description { get; set; }

    public List<AccountingEntryDto> IncomeLedgerAccountLines { get; set; } = new();

    public List<MonetaryAccountEntryDto> MonetaryAccountEntries { get; set; } = new();
}
