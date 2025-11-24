using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SaaSApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet("{tenantId}/items")]
        public async Task<IActionResult> GetAll(int tenantId)
        {
            var items = await _menuService.GetAllAsync(tenantId);
            return Ok(items);
        }

        [HttpGet("{tenantId}/item/{id}")]
        public async Task<IActionResult> GetById(int tenantId, int id)
        {
            var item = await _menuService.GetByIdAsync(id, tenantId);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost("item")]
        public async Task<IActionResult> AddItem([FromBody] MenuItemDto dto)
        {
            var id = await _menuService.AddAsync(dto);
            return Ok(new { id, message = "Item added successfully" });
        }

        [HttpPut("item/{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] MenuItemDto dto)
        {
            var success = await _menuService.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return Ok(new { message = "Item updated successfully" });
        }

        [HttpDelete("item/{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var success = await _menuService.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Item deleted successfully" });
        }
    }
}
