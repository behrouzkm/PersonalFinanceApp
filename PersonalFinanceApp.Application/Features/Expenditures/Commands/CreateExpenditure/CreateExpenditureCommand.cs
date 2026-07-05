using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Features.Expenditures.Common;

namespace PersonalFinanceApp.Application.Features.Expenditures.Commands.CreateExpenditure;

public class CreateExpenditureCommand :IRequest<Guid>
{
    public DateOnly DocumentDate { get; set; }
    public byte CurrencyId { get; set; }


    // What was it spent on, and how much for each item. Every line becomes a debit entry in the accounting document.
    public List<ExpenditureLineDto> ExpenditureLines { get; set; } = new();


    // How the expenditure was paid for. Every payment becomes a credit entry in the accounting document.
    // Who paid for it, and how much did they pay. It can split between multiple people and/or multiple cash/bank accounts.
    public List<PersonPaymentDto> PersonPayments { get; set; } = new();
    public List<MonetaryAccountPaymentDto> MonetaryAccountPayments { get; set; } = new();
}
