using System.ComponentModel.DataAnnotations;

namespace Restaurant8.Dtos.Dish
{
    public class UpdateDishRequestDto
    {
        [MaxLength(50, ErrorMessage = "Title cannot be over 50 characters")]
        public string? Title { get; set; }

        [MaxLength(200, ErrorMessage = "Anons cannot be over 200 characters")]
        public string? Anons { get; set; }

        [MaxLength(1000, ErrorMessage = "Text cannot be over 1000 characters")]
        public string? Text { get; set; }

        public string? Tags { get; set; }

        public IFormFile? Image { get; set; }
    }
}
