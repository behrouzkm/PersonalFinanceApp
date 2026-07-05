using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Common.Exceptions;

public class BusinessRuleException : Exception
{
    public string ErrorCode { get; }
    public object[] Parameters { get; }

    public BusinessRuleException(string errorCode, params object[] parameters)
        : base(errorCode)
    {
        ErrorCode = errorCode;
        Parameters = parameters;
    }
}
