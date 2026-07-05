using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Domain.Errors;

public class DomainException : Exception
{
    public string ErrorCode { get; }

    public DomainException(string errorCode) : base(errorCode) => ErrorCode = errorCode;

}
