using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITemplateService
    {
        Task<IEnumerable<TemplateDto>> GetAllAsync();
        Task<TemplateDto> GetByIdAsync(int id);
        Task<Template?> GetBySlugAsync(string slug);
        Task UpdateAsync(Template template);
        Task<Template?> GetBySubdomainAsync(string subdomain);



    }
}
