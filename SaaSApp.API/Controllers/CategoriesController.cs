using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaaSApp.Infrastructure.Data;

namespace SaaSApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("template/{templateId}")]
        public async Task<IActionResult> GetByTemplate(int templateId)
        {
            var categories = await _context.Categories
                .Where(c => c.TemplateId == templateId)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ImageUrl = c.ImageUrl
                })
                .ToListAsync();

            return Ok(categories);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }
    }
}
