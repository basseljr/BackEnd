using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITemplateFlowService
    {
        Task<Guid> SaveDraftAsync(int templateId, string customizationData);
        Task<TemplateDraft?> GetDraftAsync(Guid id);
        Task<Tenant> CreateTenantFromDraftAsync(Guid draftId, string email, string password, string plan);
    }

}
