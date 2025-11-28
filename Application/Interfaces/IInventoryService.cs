using Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryItemDto>> GetAllAsync();
        Task<InventoryItemDto?> UpdateStockAsync(int id, UpdateStockDto dto);
        Task<bool> BulkUpdateStockAsync(IEnumerable<BulkStockUpdateDto> updates);
    }
}

