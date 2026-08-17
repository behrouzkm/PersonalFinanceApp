using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.Incomes.Commands.RestoreIncome;

public class RestoreIncomeCommand : IRequest
{
    public Guid AccountingDocumentId { get; set; }
}
