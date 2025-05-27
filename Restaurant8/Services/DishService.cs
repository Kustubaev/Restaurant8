using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Restaurant8.Data;
using Restaurant8.Dtos.Comment;
using Restaurant8.Dtos.Dish;
using Restaurant8.Dtos.Query;
using Restaurant8.Interfaces;
using Restaurant8.Mappers;
using Restaurant8.Models;
using Restaurant8.Settings;
using System.Linq.Expressions;

namespace Restaurant8.Services
{
    public class DishService : IDishService
    {
        private readonly ApplicationDBContext _context;
        private readonly string _webRootUrl;

        public DishService(ApplicationDBContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _webRootUrl = appSettings.Value.WebRootUrl;
        }

        public async Task<List<GetDishSummaryDto>> GetAllSummariesAsync()
        {
            return await _context.Dishes
                .Select(dish => new GetDishSummaryDto
                {
                    Id = dish.Id,
                    Title = dish.Title,
                    Anons = dish.Anons ?? string.Empty,
                    Tags = dish.Tags ?? string.Empty,
                    Image = !string.IsNullOrEmpty(dish.Image) ? $"{_webRootUrl}/{dish.Image}" : ""
                })
                .ToListAsync();
        }

        public async Task<GetDishDto?> GetDetailsByIdAsync(int id)
        {
            return await _context.Dishes
                .Include(d => d.Comments)
                    .ThenInclude(c => c.AppUser)
                .Where(d => d.Id == id)
                .Select(dish => new GetDishDto
                {
                    Id = dish.Id,
                    Title = dish.Title,
                    Anons = dish.Anons ?? string.Empty,
                    Text = dish.Text ?? string.Empty,
                    Tags = dish.Tags ?? string.Empty,
                    Image = !string.IsNullOrEmpty(dish.Image) ? $"{_webRootUrl}/{dish.Image}" : "",
                    Comments = dish.Comments.Select(comment => new CommentDto
                    {
                        Id = comment.Id,
                        Content = comment.Content,
                        AppUserId = comment.AppUserId,
                        Username = comment.AppUser.UserName,
                        CreatedDate = comment.CreatedDate,
                        UpdatedDate = comment.UpdatedDate,
                        DishId = comment.DishId
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PaginatedResult<GetDishSummaryDto>> GetAllFilteredAsync(DishQueryDto query)
        {
            var dbQuery = _context.Dishes.AsQueryable();

            // Фильтр по строке поиска
            if (!string.IsNullOrEmpty(query.Search))
            {
                var searchLower = query.Search.ToLower();
                dbQuery = dbQuery.Where(d =>
                    d.Title.ToLower().Contains(searchLower) ||
                    d.Anons.ToLower().Contains(searchLower) ||
                    (d.Tags != null && d.Tags.ToLower().Contains(searchLower)));
            }

            // Фильтр по тегам
            if (!string.IsNullOrEmpty(query.Tags))
            {
                var tagList = query.Tags.Split(',')
                                        .Select(t => t.Trim().ToLower())
                                        .Where(t => !string.IsNullOrWhiteSpace(t))
                                        .ToList();

                dbQuery = dbQuery.Where(d => d.Tags != null &&
                    tagList.Any(tag => d.Tags.ToLower().Contains(tag)));
            }

            // Парсинг сортировки
            var (isDescending, selector) = ParseSortExpression(query.SortBy);

            IOrderedQueryable<Dish> orderedQuery = isDescending
                ? dbQuery.OrderByDescending(selector)
                : dbQuery.OrderBy(selector);

            // Подсчёт общего количества записей
            var totalCount = await orderedQuery.CountAsync();

            // Применение пагинации и маппинг в DTO
            var items = await orderedQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(dish => new GetDishSummaryDto
                {
                    Id = dish.Id,
                    Title = dish.Title,
                    Anons = dish.Anons ?? string.Empty,
                    Tags = dish.Tags ?? string.Empty,
                    Image = !string.IsNullOrEmpty(dish.Image) ? $"{_webRootUrl}/{dish.Image}" : ""
                })
                .ToListAsync();

            return new PaginatedResult<GetDishSummaryDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        private (bool IsDescending, Expression<Func<Dish, object>> Selector) ParseSortExpression(string sortBy)
        {
            var sortField = sortBy.Trim();
            bool isDescending = false;

            if (sortField.StartsWith("-"))
            {
                isDescending = true;
                sortField = sortField.Substring(1);
            }

            Expression<Func<Dish, object>> selector = sortField.ToLower() switch
            {
                "title" => d => d.Title,
                "anons" => d => d.Anons,
                "tags" => d => d.Tags,
                "id" => d => d.Id,
                _ => d => d.Title
            };

            return (isDescending, selector);
        }
    }
}