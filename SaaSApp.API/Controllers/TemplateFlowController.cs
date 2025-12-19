using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SaaSApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TemplateFlowController : ControllerBase
    {
        private readonly ITemplateFlowService _service;

        public TemplateFlowController(ITemplateFlowService service)
        {
            _service = service;
        }

        [HttpPost("save-draft")]
        public async Task<IActionResult> SaveDraft([FromBody] SaveDraftDto dto)
        {
            var id = await _service.SaveDraftAsync(dto.TemplateId, dto.CustomizationData);
            return Ok(new { draftId = id });
        }

        [HttpGet("draft/{id}")]
        public async Task<IActionResult> GetDraft(Guid id)
        {
            var draft = await _service.GetDraftAsync(id);
            if (draft == null) return NotFound();
            return Ok(draft);
        }

        [HttpPost("create-tenant-from-draft")]
        public async Task<IActionResult> CreateTenantFromDraft([FromBody] CreateTenantFromDraftDto dto)
        {
      
            var tenant = await _service.CreateTenantFromDraftAsync(dto.DraftId,dto.Email, dto.Password, dto.Plan);
            return Ok(new
            {
                tenantId = tenant.Id,
                subdomain = tenant.Subdomain
            });
        }
    }

}
