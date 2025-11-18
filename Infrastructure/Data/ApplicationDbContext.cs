using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SaaSApp.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Template>().HasData(
                new Template
                {
                    Id = 1,
                    Name = "Perfume Store",
                    Description = "Elegant template for perfume shops.",
                    PreviewImage = "/assets/templates/perfume.jpg",
                    DemoUrl = "https://demo.yoursite.com/perfume",
                    Category = "E-commerce"
                },
                new Template
                {
                    Id = 2,
                    Name = "Restaurant Menu",
                    Description = "Modern single-page layout for restaurants.",
                    PreviewImage = "/assets/templates/restaurant.jpg",
                    DemoUrl = "https://demo.yoursite.com/restaurant",
                    Category = "Food & Drink"
                }
            );

           modelBuilder.Entity<Category>()
              .HasOne(c => c.Template)
              .WithMany()
              .HasForeignKey(c => c.TemplateId)
              .OnDelete(DeleteBehavior.Cascade);

           modelBuilder.Entity<Item>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.Order)
                .WithMany(p => p.Items)
                .HasForeignKey(o => o.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }


    }
}
