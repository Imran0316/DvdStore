using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DvdStore.Models
{
    public class Payments
    {
        [Key]
        public int PaymentID { get; set; }

        public int? OrderID { get; set; }
        [ForeignKey("OrderID")]
        public Orders tbl_Orders { get; set; }

        public int? UserID { get; set; }
        [ForeignKey("UserID")]
        public Users tbl_Users { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(100)]
        public string? PaymentMethod { get; set; }  // e.g. CreditCard, JazzCash, Easypaisa

        [StringLength(50)]
        public string Status { get; set; } = "Completed"; // Default Completed
    }
}

