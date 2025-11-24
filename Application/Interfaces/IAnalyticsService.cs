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
                int tenantId,
                string period,
                DateTime? startDate,
                DateTime? endDate
            );

            Task<IEnumerable<TopItemDto>> GetTopItemsAsync(
                int tenantId,
                int limit,
                DateTime? startDate,
                DateTime? endDate
            );

            Task<IEnumerable<OrderStatusBreakdownDto>>
                GetOrderStatusBreakdownAsync(
                    int tenantId,
                    DateTime? startDate,
                    DateTime? endDate
                );

            Task<CustomerAnalyticsDto> GetCustomerAnalyticsAsync(
                int tenantId,
                int days
            );
    }
  
}
