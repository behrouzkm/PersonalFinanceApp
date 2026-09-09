using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.Common;
using PersonalFinanceApp.Application.Features.BankAccounts.Common;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.BankAccounts.Queries.GetBankAccountsList;


public class GetBankAccountsListQuery : IRequest<PaginatedList<BankAccountListItemDto>>
{
    public BankAccountType? BankAccountType { get; set; }
    public int? CurrencyId { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
