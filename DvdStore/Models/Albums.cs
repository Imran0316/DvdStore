using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DvdStore.Models
{
    public class Albums
    {
        [Key]
        public int AlbumID { get; set; }


        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public int? ArtistID { get; set; }
        [ForeignKey("ArtistID")]
        public Artists tbl_Artists { get; set; }

        [Required]
        public int? CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public Category tbl_Category { get; set; }

        [Required]
        public DateTime? ReleaseDate { get; set; }

        [Required]
        public string? Description { get; set; }  // nvarchar(max) banega

        [Required]
        [StringLength(500)]
        public string? CoverImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
