using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuItemDto>> GetAllAsync();
        Task<MenuItemDto?> GetByIdAsync(int id);
        Task<int> AddAsync(MenuItemDto dto);
        Task<bool> UpdateAsync(int id, MenuItemDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
