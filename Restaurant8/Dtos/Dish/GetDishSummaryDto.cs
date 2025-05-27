namespace Restaurant8.Dtos.Dish
{
    public class GetDishSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Anons { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public string? Image { get; set; }
    }
}
