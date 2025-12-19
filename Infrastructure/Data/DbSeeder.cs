using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SaaSApp.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Seed default tenant if it doesn't exist
            if (!await context.Users.AnyAsync(u => u.Email == "admin@test.com"))
            {
                var adminUser = new User
                {
                    FullName = "Super Admin",
                    Email = "admin@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
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
