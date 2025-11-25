using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAnalyticsService
    {
        Task<SalesSummaryDto> GetSalesSummaryAsync(
            string period,
            DateTime? startDate,
            DateTime? endDate
        );

        Task<IEnumerable<TopItemDto>> GetTopItemsAsync(
            int limit,
            DateTime? startDate,
            DateTime? endDate
        );

        Task<IEnumerable<OrderStatusBreakdownDto>>
            GetOrderStatusBreakdownAsync(
                DateTime? startDate,
                DateTime? endDate
            );

        Task<CustomerAnalyticsDto> GetCustomerAnalyticsAsync(
            int days
        );
    }
}
