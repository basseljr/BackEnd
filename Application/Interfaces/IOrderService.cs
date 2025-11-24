using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(CreateOrderRequest request);
        Task<OrderDto?> GetByIdAsync(int id, int tenantId);
        Task<IEnumerable<OrderDto>> GetByCustomerMobileAsync(string mobile, int tenantId);
        Task<IEnumerable<Order>> GetAllAsync(int tenantId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status, int tenantId);
    }
}
