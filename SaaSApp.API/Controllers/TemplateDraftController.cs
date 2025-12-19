using Application.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("[controller]")]
[Authorize]
public class TemplateDraftController : ControllerBase
{
    private readonly ITemplateDraftService _draftService;

    public TemplateDraftController(ITemplateDraftService draftService)
    {
        _draftService = draftService;
    }

    // 🔹 1) Get draft by user email (used for publish flow)
    [HttpGet("user/{email}")]
    public async Task<IActionResult> GetUserDraft(string email)
    {
        var draft = await _draftService.GetDraftByUserEmailAsync(email);

        if (draft == null)
            return NotFound(new { message = "No draft found for user" });

        return Ok(draft);
    }

    // 🔹 2) Update existing draft OR create new draft
    [HttpPost("update-or-create")]
    public async Task<IActionResult> UpdateOrCreateDraft([FromBody] SaveDraftDto dto)
    {
        try
        {
            var result = await _draftService.UpdateOrCreateDraftAsync(dto);
            return Ok(result);

        }
        catch (Exception ex) { 
            
        throw ex;

        }

    }

}
