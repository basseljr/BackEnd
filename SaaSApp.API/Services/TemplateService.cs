using Microsoft.EntityFrameworkCore;
using SaaSApp.API.DTOs;
using SaaSApp.Infrastructure.Data;

namespace SaaSApp.API.Services
{
    public class TemplateService
    {
        private readonly AppDbContext _context;

        public TemplateService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TemplateDto>> GetAllTemplatesAsync()
        {
            return await _context.Templates
                .Select(t => new TemplateDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    PreviewImage = t.PreviewImage,
                    DemoUrl = t.DemoUrl,
                    Category = t.Category
                })
                .ToListAsync();
        }

        public async Task<TemplateDto?> GetTemplateByIdAsync(int id)
        {
            return await _context.Templates
                .Where(t => t.Id == id)
                .Select(t => new TemplateDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    PreviewImage = t.PreviewImage,
                    DemoUrl = t.DemoUrl,
                    Category = t.Category
                })
                .FirstOrDefaultAsync();
        }
    }
}
