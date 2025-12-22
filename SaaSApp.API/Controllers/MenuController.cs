using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SaaSApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly TenantContext _tenantContext;
        public MenuController(IMenuService menuService, TenantContext tenantContext)
        {
            _menuService = menuService;
            _tenantContext = tenantContext;
        }

        [HttpGet("items")]
        public async Task<IActionResult> GetAll()
        {
            var items = await _menuService.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("item/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _menuService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost("item")]
        [Authorize(Roles = "Admin,Owner,Customer")]
        public async Task<IActionResult> AddItem([FromBody] MenuItemDto dto)
        {
            var id = await _menuService.AddAsync(dto);
            return Ok(new { id, message = "Item added successfully" });
        }

        [HttpPut("item/{id}")]
        [Authorize(Roles = "Admin,Owner,Customer")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] MenuItemDto dto)
        {
            var success = await _menuService.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return Ok(new { message = "Item updated successfully" });
        }

        [HttpDelete("item/{id}")]
        [Authorize(Roles = "Admin,Owner,Customer")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var success = await _menuService.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Item deleted successfully" });
        }

        [HttpPut("items/{id}/availability")]
        [Authorize(Roles = "Admin,Owner,Customer")]
        public async Task<IActionResult> ToggleAvailability(int id, [FromQuery] bool enabled)
        {
            var item = await _menuService.ToggleAvailabilityAsync(id, enabled);
            if (item == null)
            {
                // Check if it's a conflict (zero stock) or not found
                var existing = await _menuService.GetByIdAsync(id);
                if (existing == null) return NotFound();
                return Conflict(new { message = "Cannot enable item with zero stock" });
            }
            return Ok(item);
        }
    }
}
