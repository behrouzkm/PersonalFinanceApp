using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Incomes.Commands.DeleteIncome;

public class DeleteIncomeCommandValidator : AbstractValidator<DeleteIncomeCommand>
{
    public DeleteIncomeCommandValidator()
    {
        RuleFor(x => x.AccountingDocumentId)
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.Income.AccountingDocumentIdRequired);

        RuleFor(x => x.RowVersion)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.Income.RowVersionRequired);
    }

}
