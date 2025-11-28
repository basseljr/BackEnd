using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using SaaSApp.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _context;
        private readonly TenantContext _tenantContext;

        public InventoryService(AppDbContext context, TenantContext tenantContext)
        {
            _context = context;
            _tenantContext = tenantContext;
        }

        public async Task<IEnumerable<InventoryItemDto>> GetAllAsync()
        {
            var tenantId = _tenantContext.TenantId;

            var items = await _context.Items
                .Where(x => x.TenantId == tenantId)
                .Include(x => x.Category)
                .Select(x => new InventoryItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category != null ? x.Category.Name : string.Empty,
                    StockQuantity = x.StockQuantity,
                    IsAvailable = x.IsAvailable,
                    IsTrackStock = x.IsTrackStock,
                    ImageUrl = x.Image
                })
                .ToListAsync();

            return items;
        }

        public async Task<InventoryItemDto?> UpdateStockAsync(int id, UpdateStockDto dto)
        {
            var tenantId = _tenantContext.TenantId;

            var item = await _context.Items
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);

            if (item == null) return null;

            item.StockQuantity = dto.StockQuantity;

            // If tracking stock and stock is zero, disable item
            if (item.IsTrackStock && item.StockQuantity == 0)
            {
                item.IsAvailable = false;
            }

            await _context.SaveChangesAsync();

            return new InventoryItemDto
            {
                Id = item.Id,
                Name = item.Name,
                CategoryId = item.CategoryId,
                CategoryName = item.Category != null ? item.Category.Name : string.Empty,
                StockQuantity = item.StockQuantity,
                IsAvailable = item.IsAvailable,
                IsTrackStock = item.IsTrackStock,
                ImageUrl = item.Image
            };
        }

        public async Task<bool> BulkUpdateStockAsync(IEnumerable<BulkStockUpdateDto> updates)
        {
            var tenantId = _tenantContext.TenantId;
            var updateList = updates.ToList();

            // Get all item IDs from updates
            var itemIds = updateList.Select(u => u.Id).ToList();

            // Load all items that need to be updated
            var items = await _context.Items
                .Where(x => itemIds.Contains(x.Id) && x.TenantId == tenantId)
                .ToListAsync();

            // Validate all items belong to tenant
            if (items.Count != updateList.Count)
            {
                return false; // Some items not found or belong to different tenant
            }

            // Update stock for each item
            foreach (var update in updateList)
            {
                var item = items.First(x => x.Id == update.Id);

                // If isTrackStock is false, ignore stock update
                if (!item.IsTrackStock) continue;

                item.StockQuantity = update.StockQuantity;

                // If stock is zero, disable item
                if (item.StockQuantity == 0)
                {
                    item.IsAvailable = false;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}

