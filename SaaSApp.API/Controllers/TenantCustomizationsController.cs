using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class TenantCustomizationsController : ControllerBase
{
    private readonly ITenantCustomizationService _service;
    private readonly TenantContext _tenantContext;
    public TenantCustomizationsController(ITenantCustomizationService service, TenantContext tenantContext)
    {
        _service = service;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomization()
    {
        var customization = await _service.GetCurrentAsync();
        if (customization == null) return NotFound();
        return Ok(customization);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Owner,Customer")]
    public async Task<IActionResult> SaveCustomization([FromBody] TenantCustomizationDto dto)
    {
        await _service.SaveCustomizationAsync(dto);
        return Ok(new { message = "Customization saved successfully" });
    }
}
