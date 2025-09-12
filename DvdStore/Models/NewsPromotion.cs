using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DvdStore.Models
{
    public class NewsPromotion
    {
        [Key]
        public int NewsID { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [StringLength(500)]
        public string ImageUrl { get; set; }

        public DateTime PublishDate { get; set; } = DateTime.Now;

        public DateTime? ExpiryDate { get; set; }

        [Required]
        public string Type { get; set; } = "News"; // "News", "Promotion", "Event"

        public bool IsActive { get; set; } = true;

        public int? ProductID { get; set; } // Make this nullable

        // FIX: Make the navigation property virtual and nullable
        [ForeignKey("ProductID")]
        public virtual Products? Product { get; set; } // Add ? to make it nullable
    }
}