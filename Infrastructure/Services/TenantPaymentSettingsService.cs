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
    public class TenantPaymentSettingsService : ITenantPaymentSettingsService
    {
        private readonly AppDbContext _db;

        public TenantPaymentSettingsService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<TenantPaymentSettingsDto?> GetAsync(int tenantId)
        {
            var settings = await _db.TenantPaymentSettings
                .FirstOrDefaultAsync(x => x.TenantId == tenantId);

            if (settings == null) return null;

            return new TenantPaymentSettingsDto
            {
                GatewayTypeId = settings.GatewayTypeId,
                ApiKey = settings.ApiKey,
                SecretKey = settings.SecretKey
            };
        }

        public async Task SaveAsync(int tenantId, TenantPaymentSettingsDto dto)
        {
            var settings = await _db.TenantPaymentSettings
                .FirstOrDefaultAsync(x => x.TenantId == tenantId);

            if (settings == null)
            {
                settings = new TenantPaymentSettings
                {
                    TenantId = tenantId
                };

                _db.TenantPaymentSettings.Add(settings);
            }

            settings.GatewayTypeId = dto.GatewayTypeId;
            settings.ApiKey = dto.ApiKey;
            settings.SecretKey = dto.SecretKey;

            await _db.SaveChangesAsync();
        }
    }
}
