using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Features.Expenditures.Common;

namespace PersonalFinanceApp.Application.Features.Expenditures.Commands.UpdateExpenditure;

public class UpdateExpenditureCommandValidator: AbstractValidator<UpdateExpenditureCommand>
{

    public UpdateExpenditureCommandValidator()
    {
        this.ApplySharedExpenditureRules();
    }
}
