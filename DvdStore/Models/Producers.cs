using System.ComponentModel.DataAnnotations;

namespace DvdStore.Models
{
    public class Producers
    {
        [Key]
        public int ProducerID { get; set; }

        [Required]
        [StringLength(150)]
        public string ProducerName { get; set; }

        [StringLength(300)]
        public string? ContactInfo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
