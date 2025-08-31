using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DvdStore.Models
{
    public class Products
    {
        [Key]
        public int ProductID { get; set; }

        // Foreign Key → Album
        [Required]
        public int AlbumID { get; set; }
        [ForeignKey("AlbumID")]
        public Albums tbl_Albums { get; set; }
        public int? SupplierID { get; set; }
        [ForeignKey("SupplierID")]
        public Suppliers tbl_Suppliers { get; set; }
        public int? ProducerID { get; set; }
        [ForeignKey("ProducerID")]
        public Producers tbl_Producers { get; set; }

        [StringLength(100)]
        public string? SKU { get; set; }   // Stock Keeping Unit (unique code)

        public int StockQuantity { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
