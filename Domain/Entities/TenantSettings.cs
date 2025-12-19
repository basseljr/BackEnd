using Domain.Entities;

public class TenantSettings
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    public string StoreName { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? ButtonColor { get; set; }
    public string? FontFamily { get; set; }

    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Instagram { get; set; }
    public string? Facebook { get; set; }
    public string? Tiktok { get; set; }

    public string? Description { get; set; }
    public string? FooterText { get; set; }

    public string? OpeningHoursJson { get; set; }

    public Tenant? Tenant { get; set; }
}
