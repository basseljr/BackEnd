using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SaaSApp.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Seed default tenant if it doesn't exist
            if (!await context.Tenants.AnyAsync(t => t.Id == 1))
            {
                var defaultTenant = new Tenant
                {
                    Id = 1,
                    Name = "Default Tenant",
                    Subdomain = "default",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                context.Tenants.Add(defaultTenant);
            }

            // Seed default admin user if it doesn't exist
            if (!await context.Users.AnyAsync(u => u.Email == "admin@test.com"))
            {
                var adminUser = new User
                {
                    FullName = "Admin User",
                    Email = "admin@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), // Default password
                    Role = "Admin",
                    TenantId = 1,
                    CreatedAt = DateTime.UtcNow
                };
                context.Users.Add(adminUser);
            }

            await context.SaveChangesAsync();
        }
    }
}
