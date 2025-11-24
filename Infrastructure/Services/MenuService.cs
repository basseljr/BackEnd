using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SaaSApp.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class MenuService : IMenuService
    {
        private readonly AppDbContext _context;
        public MenuService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuItemDto>> GetAllAsync(int tenantId)
        {
            var items = await _context.Items
                .Include(i => i.Category)
                .Where(i => i.TenantId == tenantId)
                .ToListAsync();

            return items.Select(i => new MenuItemDto
            {
                Id = i.Id,
                TenantId = i.TenantId,
                CategoryId = i.CategoryId,
                CategoryName = i.Category?.Name ?? string.Empty,
                Name = i.Name,
                Description = i.Description,
                Price = i.Price,
                Image = i.Image,
                IsAvailable = i.IsAvailable
            });
        }

        public async Task<MenuItemDto?> GetByIdAsync(int id, int tenantId)
        {
            var i = await _context.Items
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);

            if (i == null) return null;

            return new MenuItemDto
            {
                Id = i.Id,
                TenantId = i.TenantId,
                CategoryId = i.CategoryId,
                CategoryName = i.Category?.Name ?? string.Empty,
                Name = i.Name,
                Description = i.Description,
                Price = i.Price,
                Image = i.Image,
                IsAvailable = i.IsAvailable
            };
        }

        public async Task<int> AddAsync(MenuItemDto dto)
        {
            var item = new Item
            {
                TenantId = dto.TenantId,
                CategoryId = dto.CategoryId,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Image = dto.Image,
                IsAvailable = dto.IsAvailable
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return item.Id;
        }

        public async Task<bool> UpdateAsync(int id, MenuItemDto dto)
        {
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.Id == id && i.TenantId == dto.TenantId);
            if (item == null) return false;

            item.Name = dto.Name;
            item.Description = dto.Description;
            item.Price = dto.Price;
            item.CategoryId = dto.CategoryId;
            item.Image = dto.Image;
            item.IsAvailable = dto.IsAvailable;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null) return false;

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
