namespace EcommerceAPI_AI.Entities
{
    public class PhysicalProductProfile
    {
        public int Id { get; set; }
        public double WeightKg { get; set; }
        public string Dimensions { get; set; } = string.Empty;
        public string WarehouseLocation { get; set; } = string.Empty;
    }
}
