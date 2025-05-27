namespace Restaurant8.Dtos.Comment
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public string AppUserId { get; set; } = string.Empty;
        public int? DishId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
