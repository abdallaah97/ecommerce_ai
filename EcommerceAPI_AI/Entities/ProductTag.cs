using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI_AI.Entities
{
    [Index(nameof(ProductId), nameof(TagId), IsUnique = true)]
    public class ProductTag
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;

        public int TagId { get; set; }
        [ForeignKey("TagId")]
        public Tag Tag { get; set; } = null!;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
