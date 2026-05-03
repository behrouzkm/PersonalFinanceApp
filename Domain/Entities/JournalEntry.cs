using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities;

public class JournalEntry
{
    public int Id { get; private set; }

    public DateTime Date { get; private set; }

    private readonly List<JournalLine> _lines = new();

    public IReadOnlyCollection<JournalLine> Lines => _lines;

    public JournalEntry(DateTime date) {
        Date = date;
    }

    public void AddLine(JournalLine line)
    {
        _lines.Add(line);
    }

    public decimal TotalDebit => _lines.Sum(x => x.Debit);

    public  decimal TotalCredit => _lines.Sum(x => x.Credit);

    public bool IsBalanced()
    {
        return TotalDebit == TotalCredit;
    }
}
