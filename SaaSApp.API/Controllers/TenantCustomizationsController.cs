using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class TenantCustomizationsController : ControllerBase
{
    private readonly ITenantCustomizationService _service;
    public TenantCustomizationsController(ITenantCustomizationService service) => _service = service;

    [HttpGet("{tenantId}")]
    public async Task<IActionResult> GetCustomization(int tenantId)
    {
        var customization = await _service.GetByTenantIdAsync(tenantId);
        if (customization == null) return NotFound();
        return Ok(customization);
    }

    [HttpPost]
    public async Task<IActionResult> SaveCustomization([FromBody] TenantCustomizationDto dto)
    {
        await _service.SaveCustomizationAsync(dto);
        return Ok(new { message = "Customization saved successfully" });
    }
}
