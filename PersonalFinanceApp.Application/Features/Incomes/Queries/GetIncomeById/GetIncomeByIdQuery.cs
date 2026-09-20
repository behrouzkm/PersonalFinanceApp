using MediatR;

namespace PersonalFinanceApp.Application.Features.Incomes.Queries.GetIncomeById;

public class GetIncomeByIdQuery : IRequest<IncomeDetailsDto>
{
    public Guid IncomeDocumentId { get; set; }
}
