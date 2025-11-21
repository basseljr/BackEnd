using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaaSApp.Infrastructure.Data;

[ApiController]
[Route("[controller]")]
public class MenuController : ControllerBase
{
    private readonly AppDbContext _context;
    public MenuController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("items")]
    public async Task<IActionResult> GetAllItems()
    {
        var items = await _context.Items.ToListAsync();
        return Ok(items);
    }

    [HttpGet("item/{id}")]
    public async Task<IActionResult> GetItem(int id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost("item")]
    public async Task<IActionResult> AddItem([FromBody] Item item)
    {
        _context.Items.Add(item);
        await _context.SaveChangesAsync();
        return Ok(item);
    }

    [HttpPut("item/{id}")]
    public async Task<IActionResult> UpdateItem(int id, [FromBody] Item updatedItem)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null) return NotFound();

        item.Name = updatedItem.Name;
        item.Price = updatedItem.Price;
        item.Description = updatedItem.Description;
        item.CategoryId = updatedItem.CategoryId;
        item.ImageUrl = updatedItem.ImageUrl;
        item.IsAvailable = updatedItem.IsAvailable;

        await _context.SaveChangesAsync();
        return Ok(item);
    }

    [HttpDelete("item/{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null) return NotFound();

        _context.Items.Remove(item);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
