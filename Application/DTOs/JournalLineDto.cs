namespace Application.DTOs;

public class JournalLineDto
{
    public int AccountId { get; set; }

    public decimal Debit { get; set; }

    public decimal Credit { get; set; }
}
