using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.Expenditures.Commands.RestoreExpenditure;

public class RestoreExpenditureCommand : IRequest
{
    public Guid AccountingDocumentId { get; set; }
}
