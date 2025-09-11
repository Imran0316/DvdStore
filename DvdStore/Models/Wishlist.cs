// Add to your Models folder - Wishlist.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DvdStore.Models
{
    public class Wishlist
    {
        [Key]
        public int WishlistID { get; set; }

        [Required]
        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public Users User { get; set; }

        [Required]
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public Products Product { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.Now;
    }
}