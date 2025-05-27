namespace Restaurant8.Dtos.Query
{
    public class DishQueryDto
    {
        public string? Search { get; set; }
        public string? Tags { get; set; }

        public string SortBy { get; set; } = "title";

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}