using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Features.Expenditures.Common;

namespace PersonalFinanceApp.Application.Features.Expenditures.Commands.CreateExpenditure;

public class CreateExpenditureCommandValidator : AbstractValidator<CreateExpenditureCommand>
{

    public CreateExpenditureCommandValidator()
    {
       this.ApplySharedExpenditureRules();
    }

}
