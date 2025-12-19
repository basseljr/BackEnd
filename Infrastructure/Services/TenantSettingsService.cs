using Application.DTOs;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using SaaSApp.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class TenantSettingsService : ITenantSettingsService
    {
        private readonly AppDbContext _db;

        public TenantSettingsService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<TenantSettingsDto?> GetSettingsAsync(int tenantId)
        {
            var settings = await _db.TenantSettings
                .FirstOrDefaultAsync(s => s.TenantId == tenantId);

            if (settings == null) return null;

            return new TenantSettingsDto
            {
                StoreName = settings.StoreName,
                LogoUrl = settings.LogoUrl,
                PrimaryColor = settings.PrimaryColor,
                ButtonColor = settings.ButtonColor,
                FontFamily = settings.FontFamily,
                Address = settings.Address,
                Phone = settings.Phone,
                Email = settings.Email,
                Instagram = settings.Instagram,
                Facebook = settings.Facebook,
                Tiktok = settings.Tiktok,
                Description = settings.Description,
                FooterText = settings.FooterText,
                OpeningHoursJson = settings.OpeningHoursJson
            };
        }

        public async Task<TenantSettingsDto> UpdateSettingsAsync(int tenantId, TenantSettingsDto dto)
        {
            var settings = await _db.TenantSettings.FirstOrDefaultAsync(s => s.TenantId == tenantId);

            // Create if not exists
            if (settings == null)
            {
                settings = new TenantSettings
                {
                    TenantId = tenantId
                };
                _db.TenantSettings.Add(settings);
            }

            settings.StoreName = dto.StoreName;
            settings.LogoUrl = dto.LogoUrl;
            settings.PrimaryColor = dto.PrimaryColor;
            settings.ButtonColor = dto.ButtonColor;
            settings.FontFamily = dto.FontFamily;
            settings.Address = dto.Address;
            settings.Phone = dto.Phone;
            settings.Email = dto.Email;
            settings.Instagram = dto.Instagram;
            settings.Facebook = dto.Facebook;
            settings.Tiktok = dto.Tiktok;
            settings.Description = dto.Description;
            settings.FooterText = dto.FooterText;
            settings.OpeningHoursJson = dto.OpeningHoursJson;

            await _db.SaveChangesAsync();

            return dto;
        }
    }

}
