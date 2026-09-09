using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.CashAccounts.Commands.DeleteCashAccount;

public class DeleteCashAccountCommand : IRequest
{
    public Guid CashAccountId { get; set; }


    // RowVersion is used for concurrency control to ensure that the document has not been modified by
    // another user since it was last retrieved.
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
