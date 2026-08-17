using FluentValidation;

namespace PersonalFinanceApp.Application.Features.Incomes.Queries.GetIncomesList;

public class GetIncomesListQueryValidator : AbstractValidator<GetIncomesListQuery>
{
    public GetIncomesListQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);

        RuleFor(x => x)
            .Must(x => !x.FromDate.HasValue || !x.ToDate.HasValue || x.FromDate <= x.ToDate)
            .WithMessage("FromDate must not be later than ToDate.");
    }
}
