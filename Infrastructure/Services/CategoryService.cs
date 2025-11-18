using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SaaSApp.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetByTemplateAsync(int templateId)
        {
            return await _context.Categories.Include(c => c.Items).Where(c => c.TemplateId == templateId && c.IsActive)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories .Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
