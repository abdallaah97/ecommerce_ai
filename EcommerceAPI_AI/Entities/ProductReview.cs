using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI_AI.Entities
{
    public class ProductReview
    {
        public int Id { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;
    }
}
