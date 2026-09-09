using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Commands.DeleteMoneyTransfer;

public class DeleteMoneyTransferCommandValidator : AbstractValidator<DeleteMoneyTransferCommand>
{
    public DeleteMoneyTransferCommandValidator()
    {
        RuleFor(x => x.MoneyTransferDocumentId)
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.MoneyTransfer.TransferDocumentIdRequired);

        RuleFor(x => x.RowVersion)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.MoneyTransfer.RowVersionRequired);

    }

}
