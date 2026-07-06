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
            .NotEqual((byte)0)
            .WithErrorCode(ApplicationErrorCodes.Expenditure.CurrencyRequired);

        validator.RuleFor(x => x.ExpenditureLines)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.Expenditure.LinesRequired);

        validator.RuleForEach(x => x.ExpenditureLines).ChildRules(line =>
        {
            line.RuleFor(l => l.ExpenseLedgerAccountId)
                .NotEmpty()
                .NotEqual(Guid.Empty)
                .WithErrorCode(ApplicationErrorCodes.Expenditure.ExpenseAccountRequired);

            line.RuleFor(l => l.Amount)
                .GreaterThan(0)
                .WithErrorCode(ApplicationErrorCodes.Expenditure.LineAmountMustBePositive);
        });

        validator.RuleFor(x => x)
            .Must(p=> HaveAtLeastOnePayment(p))
            .WithErrorCode(ApplicationErrorCodes.Expenditure.PaymentsRequired);


        validator.RuleForEach(x => x.MonetaryAccountPayments).ChildRules(payment =>
        {
            payment.RuleFor(p => p.MonetaryAccountId)
                .NotEmpty()
                .NotEqual(Guid.Empty)
                .WithErrorCode(ApplicationErrorCodes.Expenditure.MonetaryAccountRequired);

            payment.RuleFor(p => p.Amount)
                .GreaterThan(0)
                .WithErrorCode(ApplicationErrorCodes.Expenditure.PaymentAmountMustBePositive);
        });


        validator.RuleForEach(x => x.PersonPayments).ChildRules(payment =>
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
        var totalLineAmount = command.ExpenditureLines.Sum(l => l.Amount);
        var totalPaymentAmount = command.MonetaryAccountPayments.Sum(p => p.Amount) +
                                 command.PersonPayments.Sum(p => p.Amount);

        return totalLineAmount == totalPaymentAmount;
    }

    private static bool HaveAtLeastOnePayment(IExpenditureRequest command)
    {
        return command.PersonPayments.Any(p => p.Amount > 0) ||
                command.MonetaryAccountPayments.Any(p => p.Amount > 0);
    }
}
