using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Features.Common;

namespace PersonalFinanceApp.Application.Features.MoneyTransfers.Commands.CreateMoneyTransfer;

public class CreateMoneyTransferCommand : IRequest< Guid>
{
    public DateOnly TransferDate { get; set; }
    public Guid FromMonetaryAccountId { get; set; }
    public Guid ToMonetaryAccountId { get; set; }
    public int CurrencyId{get;set;}
    public decimal Amount{get;set;}
    public string? Description { get; set; }

}
