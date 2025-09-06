using System.ComponentModel.DataAnnotations;

namespace DvdStore.Models
{
    public class Artists
    {
        [Key]
        public int ArtistID { get; set; }


        [Required(ErrorMessage = "Name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain alphabets and spaces")]
        public string ArtistName { get; set; }

        public string Bio { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
