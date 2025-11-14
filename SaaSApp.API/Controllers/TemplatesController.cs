using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaaSApp.Infrastructure.Data;

namespace SaaSApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TemplatesController : ControllerBase
    {
        private readonly ITemplateService _templateService;
        public TemplatesController(ITemplateService templateService)
        {
            _templateService = templateService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var templates = await _templateService.GetAllAsync();
            return Ok(templates);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTemplate(int id)
        {
            var template = await _templateService.GetByIdAsync(id);                       

            if (template == null)
                return NotFound();

            return Ok(template);
        }



        [HttpPost("customize")]
        public async Task<IActionResult> SaveCustomization([FromBody] SaveCustomizationRequest request)
        {
            if (string.IsNullOrEmpty(request.Slug))
                return BadRequest("Template slug is required.");

            var template = await _templateService.GetBySlugAsync(request.Slug);
            if (template == null)
                return NotFound("Template not found.");

            template.CustomizationData = request.CustomizationData;
            await _templateService.UpdateAsync(template);

            return Ok(new { message = "Customization saved successfully." });
        }

        // GET /Templates/slug/{slug}
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var template = await _templateService.GetBySlugAsync(slug);
            if (template == null)
                return NotFound("Template not found");

            return Ok(new
            {
                slug = template.Slug,
                name = template.Name,
                //templateName = "restaurant",
                templateName = template.Category.ToLower(),
                customizationData = template.CustomizationData  // JSON string
            });
        }




    }
}
