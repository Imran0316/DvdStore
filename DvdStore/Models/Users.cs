using System.ComponentModel.DataAnnotations;

namespace DvdStore.Models
{
    public class Users
    {
        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain alphabets and spaces")]
        public string Name { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public DateTime Created_At { get; set; } = DateTime.Now;


    }
}
