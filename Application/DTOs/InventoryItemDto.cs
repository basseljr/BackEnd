namespace Application.DTOs
{
    public class InventoryItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsTrackStock { get; set; }
        public string? ImageUrl { get; set; }
    }
}

