using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DvdStore.Models
{
    public class Orders
    {
        [Key]
        public int OrderID { get; set; }

        [Required]
        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public Users tbl_Users { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending";  // Default: Pending

        [StringLength(500)]
        public string? ShippingAddress { get; set; }
    }
}
