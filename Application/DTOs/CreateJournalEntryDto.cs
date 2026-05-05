using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Application.DTOs;

public class CreateJournalEntryDto
{
    public DateTime Date {  get; set; }

    public List<JournalLineDto> Lines { get; set; } = new();
}
