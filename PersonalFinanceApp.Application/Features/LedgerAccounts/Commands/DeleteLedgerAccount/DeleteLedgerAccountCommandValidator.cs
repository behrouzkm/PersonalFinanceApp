using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Commands.DeleteLedgerAccount;

public class DeleteLedgerAccountCommandValidator : AbstractValidator<DeleteLedgerAccountCommand>
{
    public DeleteLedgerAccountCommandValidator()
    {

        RuleFor(p => p.LedgerAccountId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.Person.PersonIdRequired);


    }
}
