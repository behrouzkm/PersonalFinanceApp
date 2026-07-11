using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Features.Expenditures.Common;

namespace PersonalFinanceApp.Application.Features.Expenditures.Commands.UpdateExpenditure;

public class UpdateExpenditureCommandValidator: AbstractValidator<UpdateExpenditureCommand>
{

    public UpdateExpenditureCommandValidator()
    {
        RuleFor(x=>x.AccountingDocumentId)
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.Expenditure.AccountingDocumentIdRequired);

        RuleFor(x=>x.RowVersion)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.Expenditure.RowVersionRequired);

        this.ApplySharedExpenditureRules();
    }
}
