using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Domain.Errors;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Domain.Services;

public class LedgerBalanceValidator
{
    public void ValidateChronologicalBalance(IFundSource fundSource,
                                            IReadOnlyCollection<IledgerEntryPoint> proposedEntrySet)
    {
        if (proposedEntrySet.Any(e => e.DocumentDate < fundSource.OpeningDate))
            throw new DomainException(DomainErrors.AccountingDocument.DocumentDateCannotBeBeforeFundSourceOpeningDate);

        var floor = fundSource.CreditLimit.HasValue ? -fundSource.CreditLimit.Value : decimal.MinValue;
        var runningBalance = fundSource.InitialBalance;

        foreach (var entry in proposedEntrySet.OrderChronologically())
        {
            runningBalance += entry.Debit - entry.Credit;

            if (runningBalance < floor)
                throw new DomainException(DomainErrors.AccountingDocument.RunningBalanceBelowCreditLimit);
        }
    }
}
