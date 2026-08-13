using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Features.Common;
using PersonalFinanceApp.Application.Features.Incomes.Common;

namespace PersonalFinanceApp.Application.Features.Incomes.Commands.CreateIncome;

public class CreateIncomeCommand : IRequest<Guid>, IIncomeRequest
{
    public DateOnly DocumentDate { get; set; }

    public byte CurrencyId { get; set; }

    public string? Description { get; set; }

    public List<AccountingEntryDto> IncomeLedgerAccountLines { get; set; } = new();

    public List<MonetaryAccountEntryDto> MonetaryAccountEntries { get; set; } = new();
}
