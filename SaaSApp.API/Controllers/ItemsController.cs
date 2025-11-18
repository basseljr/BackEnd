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

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var items = await _itemService.GetByCategoryAsync(categoryId);
            return Ok(items);
        }

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
