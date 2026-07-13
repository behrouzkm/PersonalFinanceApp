using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Domain.Interfaces;

public interface IFundSource
{
    Guid Id {get;}
    Guid LedgerAccountId { get; }
    string DisplayName{get;}

    byte CurrencyId { get; }
    decimal InitialBalance { get; }
    decimal CurrentBalance { get; }
    decimal? CreditLimit { get; }

    bool CanWithdraw(decimal amount);
    void AdjustBalance(decimal amount);

    void SetInitialBalance(decimal initialBalance);
}
