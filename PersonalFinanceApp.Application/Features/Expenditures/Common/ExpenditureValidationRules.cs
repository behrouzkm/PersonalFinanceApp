using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Common.Errors;
using FluentValidation;

namespace PersonalFinanceApp.Application.Features.Expenditures.Common;

public static class ExpenditureValidationRules
{
    public static void ApplySharedExpenditureRules<T>(this AbstractValidator<T> validator)
                    where T : IExpenditureRequest
    {
        validator.RuleFor(x => x.DocumentDate)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Expenditure.DocumentDateRequired)
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithErrorCode(ApplicationErrorCodes.Expenditure.DocumentDateInFuture);

        validator.RuleFor(x => x.CurrencyId)
            .NotEmpty()
            .NotEqual(0)
            .WithErrorCode(ApplicationErrorCodes.Expenditure.CurrencyRequired);

        validator.RuleFor(x => x.ExpenditureLedgerAccountLines)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.Expenditure.LinesRequired);

        validator.RuleForEach(x => x.ExpenditureLedgerAccountLines).ChildRules(line =>
        {
            line.RuleFor(l => l.LedgerAccountId)
                .NotEmpty()
                .NotEqual(Guid.Empty)
                .WithErrorCode(ApplicationErrorCodes.Expenditure.ExpenseAccountRequired);

            line.RuleFor(l => l.Amount)
                .GreaterThan(0)
                .WithErrorCode(ApplicationErrorCodes.Expenditure.LineAmountMustBePositive);
        });

        validator.RuleFor(x => x)
            .Must(p => HaveAtLeastOnePayment(p))
            .WithErrorCode(ApplicationErrorCodes.Expenditure.PaymentsRequired);


        validator.RuleForEach(x => x.MonetaryAccountEntries).ChildRules(payment =>
        {
            payment.RuleFor(p => p.MonetaryAccountId)
                .NotEmpty()
                .NotEqual(Guid.Empty)
                .WithErrorCode(ApplicationErrorCodes.Expenditure.MonetaryAccountRequired);

            payment.RuleFor(p => p.Amount)
                .GreaterThan(0)
                .WithErrorCode(ApplicationErrorCodes.Expenditure.PaymentAmountMustBePositive);
        });


        validator.RuleForEach(x => x.PersonPaymentEntries).ChildRules(payment =>
        {
            payment.RuleFor(p => p.PersonId)
                .NotEmpty()
                .NotEqual(Guid.Empty)
                .WithErrorCode(ApplicationErrorCodes.Expenditure.PersonRequired);

            payment.RuleFor(p => p.Amount)
                .GreaterThan(0)
                .WithErrorCode(ApplicationErrorCodes.Expenditure.PaymentAmountMustBePositive);
        });

        validator.RuleFor(x => x)
            .Must(p => BeBalanced(p))
            .WithErrorCode(ApplicationErrorCodes.Expenditure.NotBalanced);

    }


    private static bool BeBalanced(IExpenditureRequest command)
    {
        var totalLineAmount = command.ExpenditureLedgerAccountLines.Sum(l => l.Amount);
        var totalPaymentAmount = command.MonetaryAccountEntries.Sum(p => p.Amount) +
                                 command.PersonPaymentEntries.Sum(p => p.Amount);

        return totalLineAmount == totalPaymentAmount;
    }

    private static bool HaveAtLeastOnePayment(IExpenditureRequest command)
    {
        return command.PersonPaymentEntries.Any(p => p.Amount > 0) ||
                command.MonetaryAccountEntries.Any(p => p.Amount > 0);
    }
}
