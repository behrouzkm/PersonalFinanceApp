using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Features.Expenditures.Common;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Application.Features.Expenditures.Commands.UpdateExpenditure;

public class UpdateExpenditureCommand : IRequest, IExpenditureRequest
{
    public Guid AccountingDocumentId { get; set; }

    // The RowVersion the client last read (e.g. from a GetExpenditureById query).
    // Used to detect if someone else edited this document in the meantime.
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateOnly DocumentDate { get; set; }
    public byte CurrencyId { get; set; }

    // What was it spent on, and how much for each item. Every line becomes a debit entry.
    public List<ExpenditureLineDto> ExpenditureLines { get; set; } = new();


    // How the expenditure was paid for. Every payment becomes a credit entry in the accounting document.
    // Who paid for it, and how much did they pay. It can split between multiple people and/or multiple cash/bank accounts.
    public List<PersonPaymentDto> PersonPayments { get; set; } = new();
    public List<MonetaryAccountPaymentDto> MonetaryAccountPayments { get; set; } = new();

    public string? Description { get; set; }

}
