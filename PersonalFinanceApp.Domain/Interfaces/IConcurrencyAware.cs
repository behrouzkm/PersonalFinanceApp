using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Domain.Interfaces;

public interface IConcurrencyAware
{
    [Timestamp]
    byte[] RowVersion { get; set; }
}
