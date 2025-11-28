using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaaSApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly TenantContext _tenantContext;

        public InventoryController(IInventoryService inventoryService, TenantContext tenantContext)
        {
            _inventoryService = inventoryService;
            _tenantContext = tenantContext;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> GetAll()
        {
            var items = await _inventoryService.GetAllAsync();
            return Ok(items);
        }

        [HttpPut("{id}/stock")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockDto dto)
        {
            var item = await _inventoryService.UpdateStockAsync(id, dto);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPut("bulk")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> BulkUpdateStock([FromBody] IEnumerable<BulkStockUpdateDto> updates)
        {
            var success = await _inventoryService.BulkUpdateStockAsync(updates);
            if (!success) return StatusCode(403, new { message = "One or more items not found or belong to different tenant" });
            return Ok(new { message = "Stock updated successfully" });
        }
    }
}

