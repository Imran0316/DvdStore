using System.ComponentModel.DataAnnotations;

namespace DvdStore.Models
{
    public class Artists
    {
        [Key]
        public int ArtistID { get; set; }

        public string ArtistName { get; set; }

        public string Bio { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
