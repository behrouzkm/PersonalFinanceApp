using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Features.Common;
using PersonalFinanceApp.Application.Features.Expenditures.Common;

namespace PersonalFinanceApp.Application.Features.Expenditures.Queries.GetExpenditureById;

public class ExpenditureDetailsDto
{
    public Guid AccountingDocumentId { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    public DateOnly DocumentDate { get; set; }
    public byte CurrencyId { get; set; }
    public string? Description { get; set; }


    public List<AccountingEntryDto> ExpenditureLedgerAccountLines { get; set; } = new();
    public List<MonetaryAccountEntryDto> MonetaryAccountEntries { get; set; } = new();
    public List<PersonPaymentDto> PersonPaymentEntries { get; set; } = new();

}
