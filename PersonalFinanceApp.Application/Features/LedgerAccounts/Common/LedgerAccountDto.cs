using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Common;

public class LedgerAccountDto
{
    public Guid Id {get;set;}
    public int AccountTypeId { get;  set; }
    public string Name { get;  set; } = string.Empty!;

    // Is this account allowed for using in AccountEntry?
    public bool IsPostingAccount { get;  set; }

    public bool HasBeenUsedInEntries { get;  set; }

    // Link to the parent
    public Guid? ParentId { get;  set; }
}
