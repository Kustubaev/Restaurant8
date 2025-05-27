using Restaurant8.Dtos.Comment;

namespace Restaurant8.Dtos.Dish
{
    public class GetDishDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Anons { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public string? Image { get; set; }
        public List<CommentDto> Comments { get; set; } = new();
    }
}