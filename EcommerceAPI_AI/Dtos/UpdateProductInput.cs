namespace EcommerceAPI_AI.Dtos
{
    public class UpdateProductInput
    {
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }

        public CreatePhysicalProfileInput? Physical { get; set; }
        public CreateDigitalProfileInput? Digital { get; set; }

        public List<int> TagIds { get; set; } = new();
    }
}

