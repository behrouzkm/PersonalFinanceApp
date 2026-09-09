using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Features.MoneyTransfers.Common;

namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Queries.GetMoneyTransferById;

public class GetMoneyTransferByIdQuery : IRequest<MoneyTransferDetailsDto>
{
    public Guid MoneyTransferDocumentId { get; set; }
}
