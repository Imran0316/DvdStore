using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DvdStore.Models
{
    public class PurchaseInvoiceDetail
    {
        [Key]
        public int PurchaseInvoiceDetailID { get; set; }

        [Required]
        public int PurchaseInvoiceID { get; set; }
        [ForeignKey("PurchaseInvoiceID")]
        public PurchaseInvoice tbl_PurchaseInvoice { get; set; }

        [Required]
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public Products tbl_Products { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCost { get; set; }
    }
}
