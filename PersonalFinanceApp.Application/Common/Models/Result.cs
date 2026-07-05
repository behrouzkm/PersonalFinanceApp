using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Common.Models;

public class Result
{

}

public class Result<T> : Result
{
    public T? Value { get; set; }
}
