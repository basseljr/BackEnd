using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
 
        public interface ITenantCustomizationService
        {
            Task<TenantCustomizationDto?> GetByTenantIdAsync(int tenantId);
            Task SaveCustomizationAsync(TenantCustomizationDto dto);
        }
    
}
