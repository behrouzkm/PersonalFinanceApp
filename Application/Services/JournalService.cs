using Application.DTOs;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Application.Services;

public class JournalService:IJournalService
{
    private readonly FinanceDbContext _context;

    public JournalService(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(CreateJournalEntryDto dto)
    {
        var entry = new JournalEntry(dto.Date);

        foreach (var line in dto.Lines)
        {
            var journalLine = new JournalLine(
                line.AccountId,
                line.Debit,
                line.Credit
             );

            entry.AddLine(journalLine);
        }

        //validation check
        if (!entry.IsBalanced())
            throw new Exception("Journal entry is not balanced");

        _context.JournalEntries.Add(entry);
        await _context.SaveChangesAsync();

        return entry.Id;
    }

}
