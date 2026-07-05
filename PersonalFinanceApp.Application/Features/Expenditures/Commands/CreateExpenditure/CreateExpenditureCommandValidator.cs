using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Expenditures.Commands.CreateExpenditure;

public class CreateExpenditureCommandValidator : AbstractValidator<CreateExpenditureCommand>
{

    public CreateExpenditureCommandValidator()
    {
        RuleFor(x => x.DocumentDate)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Expenditure.DocumentDateRequired)
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithErrorCode(ApplicationErrorCodes.Expenditure.DocumentDateInFuture);

        RuleFor(x => x.CurrencyId)
            .NotEmpty()
            .NotEqual((byte)0)
            .WithErrorCode(ApplicationErrorCodes.Expenditure.CurrencyRequired);

        RuleFor(x => x.ExpenditureLines)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.Expenditure.LinesRequired);

        RuleForEach(x => x.ExpenditureLines).ChildRules(line =>
        {
            line.RuleFor(l => l.ExpenseLedgerAccountId)
                .NotEmpty()
                .NotEqual(Guid.Empty)
                .WithErrorCode(ApplicationErrorCodes.Expenditure.ExpenseAccountRequired);

            line.RuleFor(l => l.Amount)
                .GreaterThan(0)
                .WithErrorCode(ApplicationErrorCodes.Expenditure.LineAmountMustBePositive);
        });

        RuleFor(x => x)
            .Must(HaveAtLeastOnePayment)
            .WithErrorCode(ApplicationErrorCodes.Expenditure.PaymentsRequired);


        RuleForEach(x=> x.MonetaryAccountPayments).ChildRules(payment =>
        {
            payment.RuleFor(p => p.MonetaryAccountId)
                .NotEmpty()
                .NotEqual(Guid.Empty)
                .WithErrorCode(ApplicationErrorCodes.Expenditure.MonetaryAccountRequired);

            payment.RuleFor(p => p.Amount)
                .GreaterThan(0)
                .WithErrorCode(ApplicationErrorCodes.Expenditure.PaymentAmountMustBePositive);
        });


        RuleForEach(x => x.PersonPayments).ChildRules(payment =>
        {
            payment.RuleFor(p => p.PersonId)
                .NotEmpty()
                .NotEqual(Guid.Empty)
                .WithErrorCode(ApplicationErrorCodes.Expenditure.PersonRequired);

            payment.RuleFor(p => p.Amount)
                .GreaterThan(0)
                .WithErrorCode(ApplicationErrorCodes.Expenditure.PaymentAmountMustBePositive);
        });

        RuleFor(x => x)
            .Must(BeBalanced)
            .WithErrorCode(ApplicationErrorCodes.Expenditure.NotBalanced);

    }

    private static bool HaveAtLeastOnePayment(CreateExpenditureCommand command)
    {
        return command.PersonPayments.Any(p => p.Amount > 0) ||
                command.MonetaryAccountPayments.Any(p => p.Amount > 0);
    }

    private static bool BeBalanced(CreateExpenditureCommand command)
    {
        var totalLineAmount = command.ExpenditureLines.Sum(l => l.Amount);
        var totalPaymentAmount = command.MonetaryAccountPayments.Sum(p => p.Amount) +
                                 command.PersonPayments.Sum(p => p.Amount);

        return totalLineAmount == totalPaymentAmount;
    }

}
