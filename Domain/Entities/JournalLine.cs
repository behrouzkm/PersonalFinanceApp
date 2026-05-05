using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities;

public class JournalLine
{
    public int Id { get; private set; }


    public int AccountId { get;private set; }

    public decimal Debit { get; private set; }

    public decimal Credit { get; private set; }

    public JournalLine(int accountId, decimal debit, decimal credit)
    {
        if (debit < 0 || credit < 0)
            throw new ArgumentException("Amounts cannot be negative");

        AccountId = accountId;
        Debit = debit;
        Credit = credit;
    }
}
