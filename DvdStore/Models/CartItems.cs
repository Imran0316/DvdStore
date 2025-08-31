using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DvdStore.Models
{
    public class CartItems
    {
        [Key]
        public int CartItemID { get; set; }

        [Required]
        public int CartID { get; set; }
        [ForeignKey("CartID")]
        public Cart tbl_Carts { get; set; }

        [Required]
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public Products tbl_Products { get; set; }

        public int Quantity { get; set; } = 1;
    }
}
