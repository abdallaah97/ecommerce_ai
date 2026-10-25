using EcommerceAPI_AI.Entities;

namespace EcommerceAPI_AI.Dtos
{
    public class ProductOutput
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public ProductType Type { get; set; }

        public ProductCategoryOutput? Category { get; set; }

        public PhysicalProfileOutput? Physical { get; set; }
        public DigitalProfileOutput? Digital { get; set; }

        public List<ProductReviewOutput> Reviews { get; set; } = new();

        public List<TagOutput> Tags { get; set; } = new();
    }

    public class ProductCategoryOutput
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class PhysicalProfileOutput
    {
        public int Id { get; set; }
        public double WeightKg { get; set; }
        public string Dimensions { get; set; } = string.Empty;
        public string WarehouseLocation { get; set; } = string.Empty;
    }

    public class DigitalProfileOutput
    {
        public int Id { get; set; }
        public string DownloadUrl { get; set; } = string.Empty;
        public double FileSizeMB { get; set; }
        public string FileFormat { get; set; } = string.Empty;
    }
}
