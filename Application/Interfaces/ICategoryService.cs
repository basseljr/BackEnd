using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetByTemplateAsync(int templateId);
        Task<Category?> GetByIdAsync(int id);
    }
}
