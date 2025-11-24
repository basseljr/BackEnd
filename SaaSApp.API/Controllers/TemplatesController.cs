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


        [HttpGet("domainproduction")]
        public async Task<IActionResult> GetByDomainforproduction()
        {
            var host = Request.Host.Host.ToLower();       // e.g. client1.aiw.com
            var subdomain = host.Split('.')[0];           // "client1"

            var template = await _templateService.GetBySubdomainAsync(subdomain);
            if (template == null)
                return NotFound(new { message = "Subdomain not found" });

            return Ok(new
            {
                slug = template.Slug,
                name = template.Name,
                customizationData = template.CustomizationData
            });
        }

        [HttpGet("domain")]
        public async Task<IActionResult> GetByDomain([FromQuery] string domain)
        {
            var template = await _templateService.GetBySubdomainAsync(domain);
            if (template == null) return NotFound();
            return Ok(template);
        }





    }
}
