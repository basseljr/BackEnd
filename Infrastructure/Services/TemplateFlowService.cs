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
    public class TemplateFlowService : ITemplateFlowService
    {
        private readonly AppDbContext _db;

        public TemplateFlowService(AppDbContext context)
        {
            _db = context;
        }
        public async Task<Guid> SaveDraftAsync(int templateId, string customizationData)
        {
            var draft = new TemplateDraft
            {
                TemplateId = templateId,
                CustomizationData = customizationData
            };

            _db.TemplateDrafts.Add(draft);
            await _db.SaveChangesAsync();

            return draft.Id;
        }

        public async Task<TemplateDraft?> GetDraftAsync(Guid id)
        {
            return await _db.TemplateDrafts.FirstOrDefaultAsync(t => t.Id == id);
        }


        public async Task<Tenant> CreateTenantFromDraftAsync(Guid draftId, string email, string password, string plan)
        {
            // 1. Get draft
            var draft = await _db.TemplateDrafts.FirstOrDefaultAsync(d => d.Id == draftId);
            if (draft == null) throw new Exception("Draft not found");

            // 2. Get existing preview user
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                throw new Exception("User does not exist");

            // 3. Ensure user is in preview mode
            if (user.TenantId != 5)
                throw new Exception("User already has a tenant, cannot publish again.");

            // 4. Create real tenant
            var tenant = new Tenant
            {
                Name = email.Split('@')[0],
                Subdomain = GenerateSubdomain(email),
                TemplateId = draft.TemplateId,
                IsPublished = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Tenants.Add(tenant);
            await _db.SaveChangesAsync();

            // 5. Assign user to this tenant
            user.TenantId = tenant.Id;

            // 6. Transfer draft customization → TenantCustomization
            var customization = new TenantCustomization
            {
                TenantId = tenant.Id,
                TemplateId = draft.TemplateId,
                CustomizationData = draft.CustomizationData
            };
            _db.TenantCustomizations.Add(customization);

            // 7. Create subscription
            var subscription = new Subscription
            {
                TenantId = tenant.Id,
                PlanName = plan,
                Status = "Active",
                StartDate = DateTime.UtcNow
            };
            _db.Subscriptions.Add(subscription);

            // 8. Remove draft
            _db.TemplateDrafts.Remove(draft);

            await _db.SaveChangesAsync();

            return tenant;
        }



        private string GenerateSubdomain(string email)
        {
            // Base subdomain name from email before @
            var baseName = email.Split('@')[0]
                .ToLower()
                .Replace(".", "-")
                .Replace("_", "-");

            // Ensure valid DNS characters
            baseName = new string(baseName.Where(char.IsLetterOrDigit).ToArray());

            if (string.IsNullOrWhiteSpace(baseName))
                baseName = "site";

            // Check if subdomain exists
            var existing = _db.Tenants
                .Where(t => t.Subdomain.StartsWith(baseName))
                .Select(t => t.Subdomain)
                .ToList();

            if (!existing.Contains(baseName))
                return baseName;

            // If exists → add numbers: name1, name2, name3...
            int counter = 1;
            while (true)
            {
                var candidate = $"{baseName}{counter}";
                if (!existing.Contains(candidate))
                    return candidate;

                counter++;
            }
        }



    }
}
