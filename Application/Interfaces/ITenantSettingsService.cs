using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITenantSettingsService
    {
        Task<TenantSettingsDto?> GetSettingsAsync(int tenantId);
        Task<TenantSettingsDto> UpdateSettingsAsync(int tenantId, TenantSettingsDto dto);
    }

}
