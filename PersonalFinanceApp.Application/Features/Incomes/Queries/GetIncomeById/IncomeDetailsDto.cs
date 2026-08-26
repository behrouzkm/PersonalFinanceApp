using PersonalFinanceApp.Application.Features.Common;

namespace PersonalFinanceApp.Application.Features.Incomes.Queries.GetIncomeById;

// Mirrors ExpenditureDetailsDto - reuses the same AccountingEntryDto/MonetaryAccountEntryDto
// shapes the Create/Update Income commands already use, no PersonPaymentEntries since
// Income has no person side in the current domain.
public class IncomeDetailsDto
{
    public Guid AccountingDocumentId { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    public DateOnly DocumentDate { get; set; }
    public int CurrencyId { get; set; }
    public string? Description { get; set; }

    public List<AccountingEntryDto> IncomeLedgerAccountLines { get; set; } = new();
    public List<MonetaryAccountEntryDto> MonetaryAccountEntries { get; set; } = new();
}
