using Microsoft.EntityFrameworkCore;
using Restaurant8.Data;
using Restaurant8.Dtos.Dish;
using Restaurant8.Interfaces;
using Restaurant8.Mappers;
using Restaurant8.Models;
using System.IO;
using System.Threading.Tasks;

namespace Restaurant8.Repository
{
    public class DishRepository : IDishRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IDishService _dtoService;

        public DishRepository(
            ApplicationDBContext context,
            IWebHostEnvironment env,
            IDishService dtoService)
        {
            _context = context;
            _env = env;
            _dtoService = dtoService;
        }

        public async Task<List<Dish>> GetAllAsync()
        {
            return await _context.Dishes.ToListAsync();
        }

        public async Task<Dish?> GetByIdAsync(int id)
        {
            return await _context.Dishes.FindAsync(id);
        }

        public async Task<Dish> CreateAsync(CreateDishRequestDto dto)
        {
            var dish = dto.ToDishFromCreateDTO();
            if (dto.Image != null) await SaveImageAsync(dto.Image, dish);
            
            await _context.Dishes.AddAsync(dish);
            await _context.SaveChangesAsync();
            return dish;
        }

        public async Task<Dish?> UpdateAsync(int id, UpdateDishRequestDto dto)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null) return null;

            dish.UpdateDishFromDto(dto);

            if (dto.Image != null) await UpdateImageAsync(dto.Image, dish);

            _context.Dishes.Update(dish);
            await _context.SaveChangesAsync();
            return dish;
        }

        public async Task<Dish?> DeleteAsync(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null) return null;

            if (!string.IsNullOrEmpty(dish.Image))
            {
                var filePath = Path.Combine(_env.WebRootPath, dish.Image);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();
            return dish;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Dishes.AnyAsync(d => d.Id == id);
        }

        private async Task SaveImageAsync(IFormFile file, Dish dish)
        {
            ValidateFileSizeAndExtension(file);

            var uploadsFolder = Path.Combine(_env.WebRootPath, "dish_images");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            dish.Image = Path.Combine("dish_images", uniqueFileName);
        }

        private async Task UpdateImageAsync(IFormFile file, Dish dish)
        {
            ValidateFileSizeAndExtension(file);

            if (!string.IsNullOrEmpty(dish.Image))
            {
                var oldFilePath = Path.Combine(_env.WebRootPath, dish.Image);
                if (System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);
            }

            await SaveImageAsync(file, dish);
        }

        private void ValidateFileSizeAndExtension(IFormFile file)
        {
            var maxFileSize = 2 * 1024 * 1024; // 2MB
            if (file.Length > maxFileSize)
                throw new ArgumentException("File size must be less than or equal to 2 MB.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".svg" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(ext))
                throw new ArgumentException("Invalid file format. Only .jpg, .jpeg, .png, .svg are allowed.");
        }
    }
}