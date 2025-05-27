using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Restaurant8.Dtos.Comment;
using Restaurant8.Dtos.Dish;
using Restaurant8.Models;

namespace Restaurant8.Mappers
{
    public static class DishMapper
    {

        public static GetDishDto ToDishDto(this Dish dishModel, string webRootUrl)
        {
            return new GetDishDto
            {
                Id = dishModel.Id,
                Title = dishModel.Title,
                Anons = dishModel.Anons ?? string.Empty,
                Text = dishModel.Text ?? string.Empty,
                Tags = dishModel.Tags ?? string.Empty,
                Image = !string.IsNullOrEmpty(dishModel.Image) ? $"{webRootUrl}/{dishModel.Image}" : "",
                Comments = dishModel.Comments?.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    AppUserId = c.AppUserId,
                    Username = c.AppUser?.UserName ?? "Unknown",
                    CreatedDate = c.CreatedDate,
                    UpdatedDate = c.UpdatedDate,
                    DishId = c.DishId
                }).ToList() ?? new()
            };
        }

        public static async Task<GetDishDto> ToDishDtoAsync(this Dish dishModel, IWebHostEnvironment env, UserManager<AppUser> userManager)
        {
            return new GetDishDto
            {
                Id = dishModel.Id,
                Title = dishModel.Title,
                Anons = dishModel.Anons ?? string.Empty,
                Text = dishModel.Text ?? string.Empty,
                Tags = dishModel.Tags ?? string.Empty,
                Image = !string.IsNullOrEmpty(dishModel.Image) ? $"{env.WebRootUrl()}/{dishModel.Image}" : "",
                Comments = (await Task.WhenAll(dishModel.Comments
                    .Select(c => c.ToCommentDtoWithUsernameAsync(userManager))))
                    .ToList() // <-- добавили ToList()
            };
        }

        public static Dish ToDishFromCreateDTO(this CreateDishRequestDto dto)
        {
            return new Dish
            {
                Title = dto.Title,
                Anons = dto.Anons ?? string.Empty,
                Text = dto.Text ?? string.Empty,
                Tags = dto.Tags ?? string.Empty,
                Image = string.Empty // будет заполнено после сохранения файла
            };
        }

        public static void UpdateDishFromDto(this Dish dish, UpdateDishRequestDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Title)) dish.Title = dto.Title;
            if (!string.IsNullOrEmpty(dto.Anons)) dish.Anons = dto.Anons;
            if (!string.IsNullOrEmpty(dto.Text)) dish.Text = dto.Text;
            if (!string.IsNullOrEmpty(dto.Tags)) dish.Tags = dto.Tags;
        }
    }
}