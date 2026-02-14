using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SaaSApp.API.Controllers
{
    [ApiController]
    [Route("api/tenant/payment-settings")]
    public class TenantPaymentSettingsController : ControllerBase
    {
        private readonly ITenantPaymentSettingsService _service;
        private readonly TenantContext _tenant;

        public TenantPaymentSettingsController(
            ITenantPaymentSettingsService service,
            TenantContext tenant)
        {
            _service = service;
            _tenant = tenant;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _service.GetAsync(_tenant.TenantId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Save(TenantPaymentSettingsDto dto)
        {
            await _service.SaveAsync(_tenant.TenantId, dto);
            return Ok();
        }
    }
}
