using Application.Common;
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
        private readonly TenantContext _tenantContext;
        public CategoryService(AppDbContext context, TenantContext tenantContext)
        {
            _context = context;
            _tenantContext = tenantContext;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.Items)
                .Where(c => c.TenantId == _tenantContext.TenantId && c.IsActive)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == _tenantContext.TenantId);
        }

        public async Task<int> AddAsync(Category category)
        {
            category.TenantId = _tenantContext.TenantId;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category.Id;
        }

        public async Task<bool> UpdateAsync(int id, Category updatedCategory)
        {
            var existing = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == _tenantContext.TenantId);
            if (existing == null) return false;

            existing.Name = updatedCategory.Name;
            existing.Image = updatedCategory.Image;
            existing.IsActive = updatedCategory.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == _tenantContext.TenantId);
            if (existing == null) return false;

            _context.Categories.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
