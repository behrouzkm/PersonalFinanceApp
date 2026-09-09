using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Commands.RestoreMoneyTransfer;

public class RestoreMoneyTransferCommand : IRequest
{
    public Guid MoneyTransferDocumentId { get; set; }
}
