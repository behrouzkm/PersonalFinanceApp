using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.CurrencyExchanges.Commands.DeleteCurrencyExchange;

public class DeleteCurrencyExchangeCommand : IRequest
{
    public Guid CurrencyExchangeId { get; set; }

    // RowVersion is used for concurrency control to ensure that the document has not been modified by
    // another user since it was last retrieved.
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
