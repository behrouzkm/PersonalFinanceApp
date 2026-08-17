using MediatR;

namespace PersonalFinanceApp.Application.Features.Incomes.Queries.GetIncomeById;

public class GetIncomeByIdQuery : IRequest<IncomeDetailsDto>
{
    public Guid AccountingDocumentId { get; set; }
}
