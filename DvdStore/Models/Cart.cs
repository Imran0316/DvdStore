using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DvdStore.Models
{
    public class Cart
    {
        [Key]
        public int CartID { get; set; }

        [Required]
        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public Users tbl_Users { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // One-to-Many → Cart has many CartItems
        public ICollection<CartItems> tbl_CartItems { get; set; }
    }
}
