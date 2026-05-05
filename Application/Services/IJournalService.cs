using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services;

public interface IJournalService
{
    Task<int> CreateAsync(CreateJournalEntryDto dto);
}
