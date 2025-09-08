using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DvdStore.Models
{
    public class Products
    {
        [Key]
        public int ProductID { get; set; }

        [Required]
        public int AlbumID { get; set; }
        [ForeignKey("AlbumID")]
        public Albums? tbl_Albums { get; set; }

        //  Supplier
        public int? SupplierID { get; set; }
        [ForeignKey("SupplierID")]
        public Suppliers? tbl_Suppliers { get; set; }

        // Producer
        public int? ProducerID { get; set; }
        [ForeignKey("ProducerID")]
        public Producers? tbl_Producers { get; set; }

        // Inventory aur Price Management
        [StringLength(100)]
        public string? SKU { get; set; }   // Unique Code

        public int StockQuantity { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Product Image
        [StringLength(500)]
        public string? ProductImageUrl { get; set; }

        // Trailer for Movie/Game OR Preview for Music
        [StringLength(500)]
        public string? TrailerUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
