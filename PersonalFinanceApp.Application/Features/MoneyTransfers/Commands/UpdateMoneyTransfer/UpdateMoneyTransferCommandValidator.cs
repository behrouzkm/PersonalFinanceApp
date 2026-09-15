using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Commands.UpdateMoneyTransfer;

public class UpdateMoneyTransferCommandValidator : AbstractValidator<UpdateMoneyTransferCommand>
{

    public UpdateMoneyTransferCommandValidator()
    {
        RuleFor(x => x.MoneyTransferDocumentId)
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.MoneyTransfer.TransferDocumentIdRequired);

        RuleFor(x => x.RowVersion)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.MoneyTransfer.RowVersionRequired);

        RuleFor(x => x.TransferDate)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.MoneyTransfer.TransferDateRequired)
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithErrorCode(ApplicationErrorCodes.MoneyTransfer.TransferDateInFuture);

        RuleFor(x => x.CurrencyId)
            .NotEqual(0)
            .WithErrorCode(ApplicationErrorCodes.MoneyTransfer.CurrencyRequired);

        RuleFor(x => x)
            .Must(x => x.FromLedgerAccountId != x.ToLedgerAccountId)
            .WithErrorCode(ApplicationErrorCodes.MoneyTransfer.SourceAndDestinationMustDiffer);

        RuleFor(x => x.FromLedgerAccountId)
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.MoneyTransfer.FromLedgerAccountRequired);

        RuleFor(x => x.ToLedgerAccountId)
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.MoneyTransfer.ToLedgerAccountRequired);

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithErrorCode(ApplicationErrorCodes.MoneyTransfer.TransferAmountMustBePositive);
    }
}
