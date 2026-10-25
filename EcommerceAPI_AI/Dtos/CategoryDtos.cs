namespace EcommerceAPI_AI.Dtos
{
    public class CreateCategoryInput
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class CategoryOutput
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ProductCount { get; set; }
    }
}

