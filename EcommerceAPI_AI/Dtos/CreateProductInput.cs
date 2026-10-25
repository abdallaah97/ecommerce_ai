using EcommerceAPI_AI.Entities;

namespace EcommerceAPI_AI.Dtos
{
    public class CreateProductInput
    {
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public ProductType Type { get; set; }
        public int CategoryId { get; set; }

        public CreatePhysicalProfileInput? Physical { get; set; }
        public CreateDigitalProfileInput? Digital { get; set; }

        public List<int> TagIds { get; set; } = new();
    }

    public class CreatePhysicalProfileInput
    {
        public double WeightKg { get; set; }
        public string Dimensions { get; set; } = string.Empty;
        public string WarehouseLocation { get; set; } = string.Empty;
    }

    public class CreateDigitalProfileInput
    {
        public string DownloadUrl { get; set; } = string.Empty;
        public double FileSizeMB { get; set; }
        public string FileFormat { get; set; } = string.Empty;
    }
}
