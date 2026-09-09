using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.CashAccounts.Commands.CreateCashAccount;

public class CreateCashAccountCommandValidator : AbstractValidator<CreateCashAccountCommand>
{
    public CreateCashAccountCommandValidator()
    {

        RuleFor(p => p.DisplayName)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.CashAccount.DisplayNameRequired);

        RuleFor(p => p.CashName)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.CashAccount.InvalidCashName);

        RuleFor(p => p.CashAccountNumber)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.CashAccount.InvalidCashAccountNo);

        RuleFor(p => p.ParentLedgerId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.CashAccount.InvalidParentLedgerId);

        RuleFor(p => p.OpeningDate)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.CashAccount.OpeningDateRequired)
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithErrorCode(ApplicationErrorCodes.CashAccount.OpeningDateInFuture);

        RuleFor(p => p.CreditLimit)
           .GreaterThanOrEqualTo(0)
            .WithErrorCode(ApplicationErrorCodes.CashAccount.CreditLimitMustBePositive);

        RuleFor(p => p.CurrencyId)
            .NotEmpty()
            .NotEqual(0)
            .WithErrorCode(ApplicationErrorCodes.CashAccount.CurrencyRequired);


        RuleFor(p => p.Location)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.CashAccount.LocationRequired);


    }
}
