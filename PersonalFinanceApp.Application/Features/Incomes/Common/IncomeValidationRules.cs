using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace PersonalFinanceApp.Application.Features.Incomes.Common;

public static class IncomeValidationRules
{
    public static void ApplySharedIncomeRules<T>(this FluentValidation.AbstractValidator<T> validator)
                    where T : IIncomeRequest
    {
        validator.RuleFor(x => x.DocumentDate)
            .NotEmpty().WithErrorCode(Application.Common.Errors.ApplicationErrorCodes.Income.DocumentDateRequired)
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithErrorCode(Application.Common.Errors.ApplicationErrorCodes.Income.DocumentDateInFuture);

        validator.RuleFor(x => x.CurrencyId)
            .NotEmpty()
            .NotEqual((byte)0)
            .WithErrorCode(Application.Common.Errors.ApplicationErrorCodes.Income.CurrencyRequired);

        validator.RuleFor(x => x.IncomeLedgerAccountLines)
            .NotEmpty()
            .WithErrorCode(Application.Common.Errors.ApplicationErrorCodes.Income.LinesRequired);

        validator.RuleForEach(x => x.IncomeLedgerAccountLines).ChildRules(line =>
        {
            line.RuleFor(l => l.LedgerAccountId)
                .NotEmpty()
                .NotEqual(Guid.Empty)
                .WithErrorCode(Application.Common.Errors.ApplicationErrorCodes.Income.IncomeAccountRequired);

            line.RuleFor(l => l.Amount)
                .GreaterThan(0)
                .WithErrorCode(Application.Common.Errors.ApplicationErrorCodes.Income.LineAmountMustBePositive);
        });

        validator.RuleFor(x => x.MonetaryAccountEntries)
            .NotEmpty()
            .WithErrorCode(Application.Common.Errors.ApplicationErrorCodes.Income.MonetaryAccountEntriesRequired);

        validator.RuleForEach(x => x.MonetaryAccountEntries).ChildRules(payment =>
        {
            payment.RuleFor(p => p.MonetaryLedgerAccountId)
                .NotEmpty()
                .NotEqual(Guid.Empty)
                .WithErrorCode(Application.Common.Errors.ApplicationErrorCodes.Income.MonetaryAccountEntriesRequired);

            payment.RuleFor(p => p.Amount)
                .GreaterThan(0)
                .WithErrorCode(Application.Common.Errors.ApplicationErrorCodes.Income.IncomeAmountMustBePositive);
        });

        validator.RuleFor(x => x)
            .Must(p => BeBalanced(p))
            .WithErrorCode(Application.Common.Errors.ApplicationErrorCodes.Income.NotBalanced);
    }

    private static bool BeBalanced(IIncomeRequest request)
    {
        decimal totalLineAmount = request.IncomeLedgerAccountLines?.Sum(l => l.Amount) ?? 0;
        decimal totalIncomeAmount = request.MonetaryAccountEntries?.Sum(p => p.Amount) ?? 0;

        return totalLineAmount == totalIncomeAmount;
    }
}
