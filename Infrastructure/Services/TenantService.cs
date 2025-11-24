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
    public class TenantService : ITenantService
    {
        private readonly AppDbContext _context;
        public TenantService(AppDbContext context) => _context = context;

        public async Task<TenantDto?> GetByDomainAsync(string domain)
        {
            var subdomain = domain.Split('.')[0].ToLower();

            var tenant = await _context.Tenants
                .Include(t => t.Customization)
                .ThenInclude(c => c.Template)
                .FirstOrDefaultAsync(t => t.Subdomain.ToLower() == subdomain);

            if (tenant == null) return null;

            return new TenantDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Subdomain = tenant.Subdomain,
                LogoUrl = tenant.LogoUrl,
                TemplateSlug = tenant.Customization?.Template?.Slug,
                CustomizationData = tenant.Customization?.CustomizationData
            };
        }
    }
}
