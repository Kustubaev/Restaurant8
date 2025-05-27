using Restaurant8.Models;
using Restaurant8.Dtos.Dish;

namespace Restaurant8.Interfaces
{
    public interface IDishRepository
    {
        Task<List<Dish>> GetAllAsync();
        Task<Dish?> GetByIdAsync(int id);
        Task<Dish> CreateAsync(CreateDishRequestDto dto);
        Task<Dish?> UpdateAsync(int id, UpdateDishRequestDto dto);
        Task<Dish?> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}