using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Expenditures.Commands.DeleteExpenditure;

public class DeleteExpenditureCommandValidator : AbstractValidator<DeleteExpenditureCommand>
{
    public DeleteExpenditureCommandValidator()
    {
        RuleFor(x => x.AccountingDocumentId)
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.Expenditure.AccountingDocumentIdRequired);

        RuleFor(x => x.RowVersion)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.Expenditure.RowVersionRequired);

    }

}
