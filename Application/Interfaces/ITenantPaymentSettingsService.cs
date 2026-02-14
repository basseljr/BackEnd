using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITenantPaymentSettingsService
    {
        Task<TenantPaymentSettingsDto?> GetAsync(int tenantId);
        Task SaveAsync(int tenantId, TenantPaymentSettingsDto dto);
    }
}
