using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DvdStore.Models
{
    public class Songs
    {
        [Key]
        public int SongID { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }  // song ka naam

        public TimeSpan? Duration { get; set; }  // length (optional)

        [Required]
        public string? FileUrl { get; set; }  // song ka audio file path (agar upload karna ho)

        // Foreign key for Album
        public int AlbumID { get; set; }
        [ForeignKey("AlbumID")]
        public Albums Album { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
