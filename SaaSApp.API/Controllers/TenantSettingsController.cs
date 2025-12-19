using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SaaSApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TenantSettingsController : ControllerBase
    {
        private readonly ITenantSettingsService _service;
        private readonly TenantContext _tenantContext;

        public TenantSettingsController(
            ITenantSettingsService service,
            TenantContext tenantContext)
        {
            _service = service;
            _tenantContext = tenantContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            var tenantId = _tenantContext.TenantId;
            var settings = await _service.GetSettingsAsync(tenantId);

            return Ok(settings);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSettings([FromBody] TenantSettingsDto dto)
        {
            var tenantId = _tenantContext.TenantId;
            var result = await _service.UpdateSettingsAsync(tenantId, dto);

            return Ok(result);
        }
    }

}
