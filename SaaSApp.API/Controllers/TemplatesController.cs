using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaaSApp.API.DTOs;
using SaaSApp.API.Services;
using SaaSApp.Infrastructure.Data;

namespace SaaSApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TemplatesController : ControllerBase
    {
        private readonly TemplateService _templateService;
        public TemplatesController(TemplateService templateService)
        {
            _templateService = templateService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<TemplateDto>>> GetTemplates()
        {
            var templates = await _templateService.GetAllTemplatesAsync();
            return Ok(templates);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TemplateDto>> GetTemplate(int id)
        {
            var template = await _templateService.GetTemplateByIdAsync(id);                       

            if (template == null)
                return NotFound();

            return Ok(template);
        }

    }
}
