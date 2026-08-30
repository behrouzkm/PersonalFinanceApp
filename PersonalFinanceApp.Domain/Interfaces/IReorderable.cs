using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Domain.Interfaces;

public interface IReorderable
{
    int DisplayOrder { get; }
    void SetDisplayOrder(int displayOrder);
    void IncrementDisplayOrder();
    void DecrementDisplayOrder();
}
