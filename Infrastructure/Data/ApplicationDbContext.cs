using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SaaSApp.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Core tables
        public DbSet<User> Users { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        // Multi-tenant tables
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantCustomization> TenantCustomizations { get; set; }
        public DbSet<TenantSettings> TenantSettings { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<DiscountCategory> DiscountCategories { get; set; }
        public DbSet<DiscountItem> DiscountItems { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<TemplateDraft> TemplateDrafts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed templates
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

            // ===============================
            //      TENANT RELATIONSHIPS
            // ===============================

            modelBuilder.Entity<TenantCustomization>()
                .HasOne(tc => tc.Tenant)
                .WithOne(t => t.Customization)
                .HasForeignKey<TenantCustomization>(tc => tc.TenantId)
                .OnDelete(DeleteBehavior.Restrict);       // FIXED

            modelBuilder.Entity<TenantCustomization>()
                .HasOne(tc => tc.Template)
                .WithMany()
                .HasForeignKey(tc => tc.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            // Category
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Tenant)
                .WithMany(t => t.Categories)
                .HasForeignKey(c => c.TenantId)
                .OnDelete(DeleteBehavior.Restrict);       // FIXED


            // Item
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.Tenant)
                .WithMany(t => t.Items)
                .HasForeignKey(i => i.TenantId)
                .OnDelete(DeleteBehavior.Restrict);       // FIXED

            // Orders
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Tenant)
                .WithMany(t => t.Orders)
                .HasForeignKey(o => o.TenantId)
                .OnDelete(DeleteBehavior.Restrict);       // FIXED

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Item)
                .WithMany()
                .HasForeignKey(oi => oi.ItemId)
                .OnDelete(DeleteBehavior.Restrict);


            // Discounts
            modelBuilder.Entity<Discount>()
                .HasOne(d => d.Tenant)
                .WithMany()
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.Restrict);       // FIXED

            modelBuilder.Entity<DiscountCategory>()
                .HasOne(dc => dc.Discount)
                .WithMany(d => d.DiscountCategories)
                .HasForeignKey(dc => dc.DiscountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiscountCategory>()
                .HasOne(dc => dc.Category)
                .WithMany()
                .HasForeignKey(dc => dc.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);       // FIXED

            modelBuilder.Entity<DiscountItem>()
                .HasOne(di => di.Discount)
                .WithMany(d => d.DiscountItems)
                .HasForeignKey(di => di.DiscountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiscountItem>()
                .HasOne(di => di.Item)
                .WithMany()
                .HasForeignKey(di => di.ItemId)
                .OnDelete(DeleteBehavior.Restrict);       // FIXED


            // Stock
            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Tenant)
                .WithMany()
                .HasForeignKey(s => s.TenantId)
                .OnDelete(DeleteBehavior.Restrict);       // FIXED

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Item)
                .WithOne(i => i.Stock)
                .HasForeignKey<Stock>(s => s.ItemId)
                .OnDelete(DeleteBehavior.Cascade);


            // Subscriptions
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Tenant)
                .WithMany()
                .HasForeignKey(s => s.TenantId)
                .OnDelete(DeleteBehavior.Restrict);       // FIXED

            modelBuilder.Entity<TenantSettings>()
                .HasOne(ts => ts.Tenant)
                .WithOne()
                .HasForeignKey<TenantSettings>(ts => ts.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TemplateDraft>()
               .HasKey(t => t.Id);

            modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

            modelBuilder.Entity<Tenant>()
              .HasOne(t => t.Customization)
              .WithOne(c => c.Tenant)
              .HasForeignKey<TenantCustomization>(c => c.TenantId)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tenant>()
             .HasOne(t => t.Customization)
             .WithOne(c => c.Tenant)
             .HasForeignKey<TenantCustomization>(c => c.TenantId)
             .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TemplateDraft>()
            .HasOne(d => d.User)
            .WithMany(u => u.Drafts)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        }

    }
}
