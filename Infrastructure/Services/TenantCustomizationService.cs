using Application.Common;
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
        private readonly TenantContext _tenantContext;
        public TenantCustomizationService(AppDbContext context, TenantContext tenantContext)
        {
            _context = context;
            _tenantContext = tenantContext;
        }

        public async Task<TenantCustomizationDto?> GetCurrentAsync()
        {
            var customization = await _context.TenantCustomizations
                .Include(c => c.Template)
                .FirstOrDefaultAsync(c => c.TenantId == _tenantContext.TenantId);

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
            var existing = await _context.TenantCustomizations
                .FirstOrDefaultAsync(c => c.TenantId == _tenantContext.TenantId);

            if (existing == null)
            {
                var newCustomization = new TenantCustomization
                {
                    TenantId = _tenantContext.TenantId,
                    TemplateId = dto.TemplateId,
                    CustomizationData = dto.CustomizationData
                };
                _context.TenantCustomizations.Add(newCustomization);
            }
            else
            {
                existing.CustomizationData = dto.CustomizationData;
                existing.TemplateId = dto.TemplateId;
                existing.LastModified = DateTime.UtcNow;
                _context.TenantCustomizations.Update(existing);
            }

            await _context.SaveChangesAsync();
        }
    }
}
