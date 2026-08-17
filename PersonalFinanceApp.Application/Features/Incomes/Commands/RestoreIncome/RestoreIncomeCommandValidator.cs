using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Incomes.Commands.RestoreIncome;

public class RestoreIncomeCommandValidator : AbstractValidator<RestoreIncomeCommand>
{
    public RestoreIncomeCommandValidator()
    {
        RuleFor(x => x.AccountingDocumentId)
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.Income.AccountingDocumentIdRequired);
    }

}
