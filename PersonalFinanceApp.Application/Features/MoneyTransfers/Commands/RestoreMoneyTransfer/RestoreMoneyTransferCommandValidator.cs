using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Commands.RestoreMoneyTransfer;

public class RestoreMoneyTransferCommandValidator : AbstractValidator<RestoreMoneyTransferCommand>
{
    public RestoreMoneyTransferCommandValidator()
    {
        RuleFor(x => x.MoneyTransferDocumentId)
           .NotEqual(Guid.Empty)
           .WithErrorCode(ApplicationErrorCodes.MoneyTransfer.TransferDocumentIdRequired);
    }
}
