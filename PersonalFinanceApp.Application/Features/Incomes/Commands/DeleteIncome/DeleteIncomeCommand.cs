using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.Incomes.Commands.DeleteIncome;

public class DeleteIncomeCommand : IRequest
{
    public Guid AccountingDocumentId { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
