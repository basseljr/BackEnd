namespace Application.DTOs
{
    public class TemplateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PreviewImage { get; set; }
        public string DemoUrl { get; set; }
        public string Category { get; set; }
        public string Slug { get; set; } 
        public string? CustomizationData { get; set; } 
        public string? Subdomain { get; set; }     
        public bool IsPublished { get; set; }      
    }
}

