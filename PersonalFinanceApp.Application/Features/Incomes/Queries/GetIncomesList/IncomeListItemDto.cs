namespace PersonalFinanceApp.Application.Features.Incomes.Queries.GetIncomesList;

public class IncomeListItemDto
{
    public Guid IncomeDocumentId { get; set; }
    public DateOnly DocumentDate { get; set; }
    public int CurrencyId { get; set; }
    public string? Description { get; set; }
    public decimal TotalAmount { get; set; }
    public int AttachmentCount { get; set; }
}
