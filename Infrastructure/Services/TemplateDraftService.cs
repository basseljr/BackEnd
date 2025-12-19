using Application.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SaaSApp.Infrastructure.Data;

public class TemplateDraftService : ITemplateDraftService
{
    private readonly AppDbContext _db;

    public TemplateDraftService(AppDbContext db)
    {
        _db = db;
    }

    // 🔹 Get existing draft using linked user → tenant id
    public async Task<TemplateDraft?> GetDraftByUserEmailAsync(string email)
    {
        string emailcropped = email?
         .Split(',', StringSplitOptions.RemoveEmptyEntries)
         .FirstOrDefault()
         ?.Trim()
         ?? string.Empty;
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == emailcropped);

        if (user == null || user.TenantId != 5)
            return null;

        return await _db.TemplateDrafts
            .FirstOrDefaultAsync(d => d.UserId == user.Id);
    }


    // 🔹 Create or update draft
    public async Task<TemplateDraft> UpdateOrCreateDraftAsync(SaveDraftDto dto)
    {
        string email = dto.Email?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault()
            ?.Trim()
            ?? string.Empty;
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || user.TenantId != 5)
            throw new Exception("Invalid preview user");

        var existing = await _db.TemplateDrafts.FirstOrDefaultAsync(d => d.UserId == user.Id);
        //ok here check the recusrion
        if (existing != null)
        {
            existing.CustomizationData = dto.CustomizationData;
            existing.LastModified = DateTime.Now;
            await _db.SaveChangesAsync();
            return existing;
        }

        var newDraft = new TemplateDraft
        {
            Id = Guid.NewGuid(),
            TemplateId = dto.TemplateId,
            CustomizationData = dto.CustomizationData,
            UserId = user.Id,
            CreatedAt = DateTime.Now,
            LastModified = DateTime.Now
        };

        _db.TemplateDrafts.Add(newDraft);
        await _db.SaveChangesAsync();
        return newDraft;
    }


}
