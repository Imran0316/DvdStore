using System.ComponentModel.DataAnnotations;

namespace DvdStore.Models
{
    public class Suppliers
    {
        [Key]
        public int SupplierID { get; set; }

        [Required]
        [StringLength(150)]
        public string SupplierName { get; set; }

        [StringLength(300)]
        public string? ContactInfo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
