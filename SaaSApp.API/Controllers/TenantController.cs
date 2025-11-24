using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;
    public TenantsController(ITenantService tenantService) => _tenantService = tenantService;

    [HttpGet("by-domain")]
    public async Task<IActionResult> GetByDomain([FromQuery] string domain)
    {
        var tenant = await _tenantService.GetByDomainAsync(domain);
        if (tenant == null)
            return NotFound(new { message = "Tenant not found" });
        return Ok(tenant);
    }
}
