// Models/HeroImage.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DvdStore.Models
{
    public class HeroImage
    {
        [Key]
        public int HeroImageID { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Subtitle { get; set; }

        //[Required]
        [StringLength(500)]
        public string ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string ButtonText { get; set; } = "Explore Now";

        [StringLength(200)]
        public string ButtonLink { get; set; } = "/Products";
    }
}