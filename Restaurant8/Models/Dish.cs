using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant8.Models
{
    [Table("Dishes")]
    public class Dish
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Anons { get; set; }

        public string? Text { get; set; }

        public string? Tags { get; set; }

        public string Image { get; set; } = string.Empty;


        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
