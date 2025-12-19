using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SaaSApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        // NEW multi-tenant friendly endpoint
        // Supports: GET /Items?categoryId=2
        [HttpGet]
        public async Task<IActionResult> GetItems([FromQuery] int categoryId)
        {
            // categoryId MUST be provided because we do not have GetAllAsync
            if (categoryId <= 0)
            {
                return BadRequest("categoryId is required");
            }

            var items = await _itemService.GetByCategoryAsync(categoryId);
            return Ok(items);
        }

        // GET /Items/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _itemService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }
    }
}
