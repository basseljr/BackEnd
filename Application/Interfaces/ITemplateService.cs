using Application.DTOs;
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
    }
}
