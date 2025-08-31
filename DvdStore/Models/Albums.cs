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

        public int? ArtistID { get; set; }
        [ForeignKey("ArtistID")]
        public Artists tbl_Artists { get; set; }

        public int? CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public Category tbl_Category { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public string? Description { get; set; }  // nvarchar(max) banega

        [StringLength(500)]
        public string? CoverImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
