using Application.DTOs;
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
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<int> AddAsync(Category category);
        Task<bool> UpdateAsync(int id, Category updatedCategory);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateOrderAsync(IEnumerable<CategoryOrderDto> orderUpdates);
        Task<CategoryDto?> UpdateAvailabilityAsync(int id, bool enabled);
    }
}
