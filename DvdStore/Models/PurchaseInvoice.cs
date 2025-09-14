using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DvdStore.Models
{
    public class PurchaseInvoice
    {
        [Key]
        public int PurchaseInvoiceID { get; set; }

        [Required]                          // ID zaroori hai
        public int SupplierID { get; set; }

        [ForeignKey("SupplierID")]
        public Suppliers? tbl_Suppliers { get; set; }   

        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalAmount { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [ValidateNever]
        public ICollection<PurchaseInvoiceDetail>? InvoiceDetails { get; set; } 
    }

}
