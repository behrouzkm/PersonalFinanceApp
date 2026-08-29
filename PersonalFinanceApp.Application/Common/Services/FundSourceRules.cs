using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;

namespace PersonalFinanceApp.Application.Common.Services;
public static class FundSourceRules
{
    public static void ValidateCreditLimit(decimal? creditLimit, decimal initialBalance)
    {
        if (creditLimit.HasValue && initialBalance < 0 && creditLimit.Value < Math.Abs(initialBalance))
            throw new BusinessRuleException(ApplicationErrorCodes.FundSource.InitialDebtExceedsCreditLimit);
    }
}
