using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.Expenditures.Queries.GetExpenditureById;

public class GetExpenditureByIdQuery : IRequest<ExpenditureDetailsDto>
{
    public Guid AccountingDocumentId {get;set;}
}
