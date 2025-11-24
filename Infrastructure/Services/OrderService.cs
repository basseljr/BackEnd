using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SaaSApp.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        public OrderService(AppDbContext context) => _context = context;

        public async Task<int> CreateOrderAsync(CreateOrderRequest request)
        {
            var order = new Order
            {
                TenantId = request.TenantId,
                CustomerName = request.CustomerName,
                Email = request.Email,
                Mobile = request.Mobile,
                Mode = request.Mode,
                Total = request.Total,
                Status = "Pending",
                Items = request.Items.Select(i => new OrderItem
                {
                    ItemId = i.ItemId,
                    ItemName = i.ItemName,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order.Id;
        }

        public async Task<OrderDto?> GetByIdAsync(int id, int tenantId)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.TenantId == tenantId);

            if (order == null) return null;

            return new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                Email = order.Email,
                Mobile = order.Mobile,
                Mode = order.Mode,
                Total = order.Total,
                Status = order.Status,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    ItemName = i.ItemName,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };
        }

        public async Task<IEnumerable<OrderDto>> GetByCustomerMobileAsync(string mobile, int tenantId)
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.Mobile == mobile && o.TenantId == tenantId)
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                Email = o.Email,
                Mobile = o.Mobile,
                Mode = o.Mode,
                Total = o.Total,
                Status = o.Status,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ItemName = i.ItemName,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            });
        }

        public async Task<IEnumerable<Order>> GetAllAsync(int tenantId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.TenantId == tenantId)
                .OrderByDescending(o => o.Id)
                .ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status, int tenantId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.TenantId == tenantId);
            if (order == null) return false;

            order.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
