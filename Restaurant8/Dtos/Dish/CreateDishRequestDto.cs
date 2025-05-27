using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Restaurant8.Dtos.Dish
{
    public class CreateDishRequestDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(50, ErrorMessage = "Title cannot be over 50 characters")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Anons { get; set; }

        [MaxLength(1000)]
        public string? Text { get; set; }

        public string? Tags { get; set; }

        public IFormFile? Image { get; set; }
    }
}