using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.BankAccounts.Commands.ReorderBankAccount;

public class ReorderBankAccountCommand : IRequest
{
    public Guid BankAccountId { get; set; }
    public int NewDisplayOrder { get; set; }
}
