using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Common.Exceptions;

public class NotFoundException : Exception
{

    public string ErrorCode {get; }
    public string EntityName { get; }
    public object Key { get; }

    public NotFoundException(string entityName, object key)
        : base($"Entity \"{entityName}\" ({key}) was not found.")
    {
        ErrorCode = "ApplicationErrorCodes.Common.NotFound";
        EntityName = entityName;
        Key = key;
    }
}
