using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Features.Incomes.Common;

namespace PersonalFinanceApp.Application.Features.Incomes.Commands.UpdateIncome;

public class UpdateIncomeCommandValidationRule : AbstractValidator<UpdateIncomeCommand>
{

    public UpdateIncomeCommandValidationRule()
    {
        this.ApplySharedIncomeRules();
    }
}
