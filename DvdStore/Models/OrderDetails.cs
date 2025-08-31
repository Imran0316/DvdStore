using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DvdStore.Models
{
    public class OrderDetails
    {

        [Key]
        public int OrderDetailID { get; set; }

        [Required]
        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        public Orders tbl_Orders { get; set; }

        [Required]
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public Products tbl_Products { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
    }
}
