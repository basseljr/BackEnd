using Application.DTOs;
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
        Task<OrderDto?> GetByIdAsync(int id);
        Task<IEnumerable<OrderDto>> GetByCustomerMobileAsync(string mobile);

    }
}
