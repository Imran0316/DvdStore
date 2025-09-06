using System.ComponentModel.DataAnnotations;

namespace DvdStore.Models
{
    public class Producers
    {
        [Key]
        public int ProducerID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain alphabets and spaces")]
        public string ProducerName { get; set; }


        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be exactly 11 digits")]
        public string? ContactInfo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
