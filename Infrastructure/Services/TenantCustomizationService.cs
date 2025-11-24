using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SaaSApp.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{

    public class TenantCustomizationService : ITenantCustomizationService
    {
        private readonly AppDbContext _context;
        public TenantCustomizationService(AppDbContext context) => _context = context;

        public async Task<TenantCustomizationDto?> GetByTenantIdAsync(int tenantId)
        {
            var customization = await _context.TenantCustomizations.Include(c => c.Template).FirstOrDefaultAsync(c => c.TenantId == tenantId);

            if (customization == null) return null;

            return new TenantCustomizationDto
            {
                TenantId = customization.TenantId,
                TemplateId = customization.TemplateId,
                CustomizationData = customization.CustomizationData
            };
        }

        public async Task SaveCustomizationAsync(TenantCustomizationDto dto)
        {
            var existing = await _context.TenantCustomizations.FirstOrDefaultAsync(c => c.TenantId == dto.TenantId);

            if (existing == null)
            {
                var newCustomization = new TenantCustomization
                {
                    TenantId = dto.TenantId,
                    TemplateId = dto.TemplateId,
                    CustomizationData = dto.CustomizationData
                };
                _context.TenantCustomizations.Add(newCustomization);
            }
            else
            {
                existing.CustomizationData = dto.CustomizationData;
                existing.LastModified = DateTime.UtcNow;
                _context.TenantCustomizations.Update(existing);
            }

            await _context.SaveChangesAsync();
        }
    }
}
