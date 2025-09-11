using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DvdStore.Models
{
    public class ProductReviews
    {
        [Key]
        public int ReviewID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        public Products Product { get; set; }  // This should be singular

        [Required]
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public Users User { get; set; }  // This should be singular

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(500)]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsApproved { get; set; } = true;
    }
}