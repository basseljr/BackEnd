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
    public class ItemService : IItemService
    {
        private readonly AppDbContext _context;
        private readonly TenantContext _tenantContext;
        public ItemService(AppDbContext context, TenantContext tenantContext)
        {
            _context = context;
            _tenantContext = tenantContext;
        }

        public async Task<IEnumerable<Item>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Items.Where(i => i.CategoryId == categoryId && i.TenantId == _tenantContext.TenantId && i.IsAvailable).ToListAsync();
        }

        public async Task<Item?> GetByIdAsync(int id)
        {
            return await _context.Items
                .FirstOrDefaultAsync(i => i.Id == id && i.TenantId == _tenantContext.TenantId);
        }
    }
}
