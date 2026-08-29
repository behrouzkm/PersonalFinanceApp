using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PersonalFinanceApp.Application.Common.Errors;

namespace PersonalFinanceApp.Application.Features.Persons.Commands.CreatePerson;

public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
{
    public CreatePersonCommandValidator()
    {

        RuleFor(p => p.DisplayName)
            .NotEmpty()
            .WithErrorCode(ApplicationErrorCodes.Person.DisplayNameRequired);

        RuleFor(p => p.ParentLedgerId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithErrorCode(ApplicationErrorCodes.Person.InvalidParentLedgerId);

        RuleFor(p=>p.OpeningDate)
            .NotEmpty().WithErrorCode(ApplicationErrorCodes.Person.OpeningDateRequired)
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithErrorCode(ApplicationErrorCodes.Person.OpeningDateInFuture);

        RuleFor(p => p.CreditLimit)
           .Must(x => x is null || x >= 0)
            .WithErrorCode(ApplicationErrorCodes.Person.InvalidCreditLimit);

        RuleFor(p => p.CurrencyId)
            .NotEmpty()
            .NotEqual(0)
            .WithErrorCode(ApplicationErrorCodes.Person.CurrencyRequired);

        RuleFor(x => x.Email)
            .MaximumLength(320)
            .WithErrorCode(ApplicationErrorCodes.Person.EmailAddressIsTooLong);

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(r => !string.IsNullOrWhiteSpace(r.Email))
            .WithErrorCode(ApplicationErrorCodes.Person.InvalidEmailAddress);

        RuleFor(x => x.MobileNumber)
            .MaximumLength(20)
            .WithErrorCode(ApplicationErrorCodes.Person.MobileNumberIsTooLong);

        RuleFor(x => x.TelNumber)
             .MaximumLength(20)
             .WithErrorCode(ApplicationErrorCodes.Person.TelNumberIsTooLong);

    }
}
