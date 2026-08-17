using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Expenditures.Commands.RestoreExpenditure;

public class RestoreExpenditureCommandValidator : AbstractValidator<RestoreExpenditureCommand>
{
    public RestoreExpenditureCommandValidator()
    {
        RuleFor(x=> x.AccountingDocumentId)
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.Expenditure.AccountingDocumentIdRequired);
    }
}
