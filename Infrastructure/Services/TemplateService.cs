using Application.DTOs;
using Microsoft.EntityFrameworkCore;
using SaaSApp.Infrastructure.Data;
using Application.Interfaces;
using Domain.Entities;

namespace SaaSApp.Infrastructure.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly AppDbContext _context;

        public TemplateService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TemplateDto>> GetAllAsync()
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

        public async Task<TemplateDto?> GetByIdAsync(int id)
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

        public async Task<Template?> GetBySlugAsync(string slug)
        {
            return await _context.Templates.FirstOrDefaultAsync(t => t.Slug == slug);
        }

        public async Task UpdateAsync(Template template)
        {
            _context.Templates.Update(template);
            await _context.SaveChangesAsync();
        }

    }
}
