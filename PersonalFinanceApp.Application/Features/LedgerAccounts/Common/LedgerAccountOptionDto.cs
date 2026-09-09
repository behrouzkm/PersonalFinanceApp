using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.LedgerAccounts.Common;

public class LedgerAccountOptionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid ParentId { get; set; }
}
