namespace PersonalFinanceApp.Application.Features.Incomes.Queries.GetIncomesList;

public class IncomeListItemDto
{
    public Guid AccountingDocumentId { get; set; }
    public DateOnly DocumentDate { get; set; }
    public byte CurrencyId { get; set; }
    public string? Description { get; set; }
    public decimal TotalAmount { get; set; }
}
