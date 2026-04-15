using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebProject.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Cart")]
        public int CartId { get; set; }
        public virtual Cart Cart { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; } = 1;

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}