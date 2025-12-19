using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class TenantController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    // The endpoint your frontend expects:
    // GET /Tenant/resolve?host=<hostname>
    [HttpGet("resolve")]
    public async Task<IActionResult> Resolve([FromQuery] string host)
    {
        var tenant = await _tenantService.GetByDomainAsync(host);

        if (tenant == null)
            return NotFound(new { message = "Tenant not found" });

        return Ok(new { tenantId = tenant.Id });
    }

    [HttpGet("by-subdomain/{sub}")]
    public async Task<IActionResult> GetBySubdomain(string sub)
    {
        var tenant = await _tenantService.GetBySubdomainAsync(sub);

        if (tenant == null)
            return NotFound(new { message = "Tenant not found" });

        return Ok(new
        {
            tenantId = tenant.Id,
            subdomain = tenant.Subdomain,
            templateId = tenant.TemplateId,
            customizationData = tenant.Customization?.CustomizationData ?? "{}"

        });
    }

}
