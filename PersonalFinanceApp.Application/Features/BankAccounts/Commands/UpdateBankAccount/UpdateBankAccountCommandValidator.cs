using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.BankAccounts.Commands.UpdateBankAccount;

public class UpdateBankAccountCommandValidator : AbstractValidator<UpdateBankAccountCommand>
{
    public UpdateBankAccountCommandValidator()
    {

        RuleFor(p => p.BankAccountId)
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.BankAccount.BankAccountIdRequired);

        RuleFor(p => p.DisplayName)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.BankAccount.DisplayNameRequired);

        RuleFor(p => p.BankName)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.BankAccount.InvalidBankName);

        RuleFor(p => p.BankAccountNumber)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.BankAccount.InvalidBankAccountNo);

        RuleFor(p => p.ParentLedgerId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.BankAccount.InvalidParentLedgerId);

        RuleFor(p => p.OpeningDate)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.BankAccount.OpeningDateRequired)
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithErrorCode(ApplicationErrorCodes.BankAccount.OpeningDateInFuture);

        RuleFor(p => p.CreditLimit)
           .GreaterThanOrEqualTo(0)
            .WithErrorCode(ApplicationErrorCodes.BankAccount.CreditLimitMustBePositive);

        RuleFor(p => p.CurrencyId)
            .NotEmpty()
            .NotEqual(0)
            .WithErrorCode(ApplicationErrorCodes.BankAccount.CurrencyRequired);

    }
}
