using Restaurant8.Dtos.Dish;
using Restaurant8.Dtos.Query;
using Restaurant8.Models;

namespace Restaurant8.Interfaces
{
    public interface IDishService
    {
        Task<PaginatedResult<GetDishSummaryDto>> GetAllFilteredAsync(DishQueryDto query);
        Task<List<GetDishSummaryDto>> GetAllSummariesAsync();
        Task<GetDishDto?> GetDetailsByIdAsync(int id);
    }
}