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

        // List-level: the whole collection must not be empty.
        validator.RuleFor(x => x.MonetaryAccountEntries)
            .NotEmpty()
            .WithErrorCode(Application.Common.Errors.ApplicationErrorCodes.Income.MonetaryAccountEntriesRequired);

        validator.RuleForEach(x => x.MonetaryAccountEntries).ChildRules(payment =>
        {
            // Item-level: this one entry's own account reference must not be empty -
            // a distinct failure from the list being empty, so a client can tell them apart.
            payment.RuleFor(p => p.MonetaryAccountId)
                .NotEmpty()
                .NotEqual(Guid.Empty)
                .WithErrorCode(Application.Common.Errors.ApplicationErrorCodes.Income.MonetaryAccountRequired);

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
