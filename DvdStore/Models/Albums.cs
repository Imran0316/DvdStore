using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DvdStore.Models
{
    public class Albums
    {
        [Key]
        public int AlbumID { get; set; }

        [Required(ErrorMessage = "Album title is required")]
        [StringLength(200)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Artist is required")]
        public int ArtistID { get; set; }
        [ForeignKey("ArtistID")]
        public Artists? tbl_Artists { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public Category? tbl_Category { get; set; }

        [Required(ErrorMessage = "Release date is required")]
        public DateTime ReleaseDate { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        // Optional, kyunki ye controller me file upload ke through set hota hai
        [StringLength(500)]
        public string? CoverImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
