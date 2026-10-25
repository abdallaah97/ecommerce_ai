using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI_AI.Entities
{
    [Index(nameof(Sku), IsUnique = true)]
    [Index(nameof(PhysicalProfileId), IsUnique = true)]
    [Index(nameof(DigitalProfileId), IsUnique = true)]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ProductType Type { get; set; }

        public int? PhysicalProfileId { get; set; }
        [ForeignKey("PhysicalProfileId")]
        public PhysicalProductProfile? PhysicalProfile { get; set; }

        public int? DigitalProfileId { get; set; }
        [ForeignKey("DigitalProfileId")]
        public DigitalProductProfile? DigitalProfile { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!;

        public ICollection<ProductReview> Reviews { get; set; }

        public ICollection<ProductTag> ProductTags { get; set; }
    }

    public enum ProductType
    {
        Physical = 1,
        Digital = 2
    }
}
