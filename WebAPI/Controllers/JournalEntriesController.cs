using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JournalEntriesController : Controller
{
    private readonly IJournalService _service;

    public JournalEntriesController(IJournalService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateJournalEntryDto dto)
    {
        var id = await _service.CreateAsync(dto);

        return Ok(new {Id =  id});
    }
}
