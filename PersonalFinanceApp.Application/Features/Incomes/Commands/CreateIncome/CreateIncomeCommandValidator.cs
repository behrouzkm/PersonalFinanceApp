using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Features.Incomes.Common;

namespace PersonalFinanceApp.Application.Features.Incomes.Commands.CreateIncome;

public class CreateIncomeCommandValidator : AbstractValidator<CreateIncomeCommand>
{

    public CreateIncomeCommandValidator()
    {
        this.ApplySharedIncomeRules();
    }
}
