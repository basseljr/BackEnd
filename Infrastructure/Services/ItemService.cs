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
        public ItemService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Item>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Items .Where(i => i.CategoryId == categoryId && i.IsAvailable).ToListAsync();
        }

        public async Task<Item?> GetByIdAsync(int id)
        {
            return await _context.Items.FindAsync(id);
        }
    }
}
