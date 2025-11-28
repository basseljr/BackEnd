using Application.Common;
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
        private readonly TenantContext _tenantContext;
        public MenuService(AppDbContext context, TenantContext tenantContext)
        {
            _context = context;
            _tenantContext = tenantContext;
        }

        public async Task<IEnumerable<MenuItemDto>> GetAllAsync()
        {
            var items = await _context.Items
                .Include(i => i.Category)
                .Where(i => i.TenantId == _tenantContext.TenantId)
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
                IsAvailable = i.IsAvailable,
                StockQuantity = i.StockQuantity,
                DiscountPercentage = i.DiscountPercentage,
                FinalPrice = i.FinalPrice,
                IsTrackStock = i.IsTrackStock
            });
        }

        public async Task<MenuItemDto?> GetByIdAsync(int id)
        {
            var i = await _context.Items
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenantContext.TenantId);

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
                IsAvailable = i.IsAvailable,
                StockQuantity = i.StockQuantity,
                DiscountPercentage = i.DiscountPercentage,
                FinalPrice = i.FinalPrice,
                IsTrackStock = i.IsTrackStock
            };
        }

        public async Task<int> AddAsync(MenuItemDto dto)
        {
            var item = new Item
            {
                TenantId = _tenantContext.TenantId,
                CategoryId = dto.CategoryId,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Image = dto.Image,
                IsAvailable = dto.IsAvailable,
                StockQuantity = dto.StockQuantity,
                DiscountPercentage = dto.DiscountPercentage,
                IsTrackStock = dto.IsTrackStock
            };

            // If tracking stock and stock is zero, disable item
            if (item.IsTrackStock && item.StockQuantity == 0)
            {
                item.IsAvailable = false;
            }

            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return item.Id;
        }

        public async Task<bool> UpdateAsync(int id, MenuItemDto dto)
        {
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.Id == id && i.TenantId == _tenantContext.TenantId);
            if (item == null) return false;

            item.Name = dto.Name;
            item.Description = dto.Description;
            item.Price = dto.Price;
            item.CategoryId = dto.CategoryId;
            item.Image = dto.Image;
            item.IsAvailable = dto.IsAvailable;
            item.StockQuantity = dto.StockQuantity;
            item.DiscountPercentage = dto.DiscountPercentage;
            item.IsTrackStock = dto.IsTrackStock;

            // If tracking stock and stock is zero, disable item
            if (item.IsTrackStock && item.StockQuantity == 0)
            {
                item.IsAvailable = false;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.Id == id && i.TenantId == _tenantContext.TenantId);
            if (item == null) return false;

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<MenuItemDto?> ToggleAvailabilityAsync(int id, bool enabled)
        {
            var item = await _context.Items
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenantContext.TenantId);

            if (item == null) return null;

            // If tracking stock and stock is zero, cannot enable
            if (item.IsTrackStock && item.StockQuantity == 0 && enabled)
            {
                // Return null to indicate conflict - controller will handle 409
                return null;
            }

            item.IsAvailable = enabled;
            await _context.SaveChangesAsync();

            return new MenuItemDto
            {
                Id = item.Id,
                TenantId = item.TenantId,
                CategoryId = item.CategoryId,
                CategoryName = item.Category?.Name ?? string.Empty,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Image = item.Image,
                IsAvailable = item.IsAvailable,
                StockQuantity = item.StockQuantity,
                DiscountPercentage = item.DiscountPercentage,
                FinalPrice = item.FinalPrice,
                IsTrackStock = item.IsTrackStock
            };
        }
    }
}
